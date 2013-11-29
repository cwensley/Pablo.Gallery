using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.Routing;

namespace Pablo.Gallery.Logic
{
	public class NamespaceHttpControllerSelector : IHttpControllerSelector
	{
		const string NamespaceKey = "version";
		const string ControllerKey = "controller";

		readonly HttpConfiguration _configuration;
		readonly Lazy<Dictionary<string, HttpControllerDescriptor>> _controllers;
		readonly HashSet<string> _duplicates;
		readonly int namespaceOffset;

		public NamespaceHttpControllerSelector(HttpConfiguration config, int namespaceOffset = 0)
		{
			this.namespaceOffset = namespaceOffset;
			_configuration = config;
			_duplicates = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			_controllers = new Lazy<Dictionary<string, HttpControllerDescriptor>>(InitializeControllerDictionary);
		}

		Dictionary<string, HttpControllerDescriptor> InitializeControllerDictionary()
		{
			var dictionary = new Dictionary<string, HttpControllerDescriptor>(StringComparer.OrdinalIgnoreCase);

			// Create a lookup table where key is "namespace.controller". The value of "namespace" is the last
			// segment of the full namespace. For example:
			// MyApplication.Controllers.V1.ProductsController => "V1.Products"
			IAssembliesResolver assembliesResolver = _configuration.Services.GetAssembliesResolver();
			IHttpControllerTypeResolver controllersResolver = _configuration.Services.GetHttpControllerTypeResolver();

			ICollection<Type> controllerTypes = controllersResolver.GetControllerTypes(assembliesResolver);

			foreach (Type t in controllerTypes)
			{
				var segments = t.Namespace.Split(Type.Delimiter);
				if ((segments.Length < namespaceOffset + 1) || !t.Name.EndsWith(DefaultHttpControllerSelector.ControllerSuffix, StringComparison.Ordinal))
					continue;

				// For the dictionary key, strip "Controller" from the end of the type name.
				// This matches the behavior of DefaultHttpControllerSelector.
				var controllerName = t.Name.Remove(t.Name.Length - DefaultHttpControllerSelector.ControllerSuffix.Length);

				var key = String.Format(CultureInfo.InvariantCulture, "{0}.{1}", segments[segments.Length - 1 - namespaceOffset], controllerName);

				// Check for duplicate keys.
				if (dictionary.Keys.Contains(key))
				{
					_duplicates.Add(key);
				}
				else
				{
					dictionary[key] = new HttpControllerDescriptor(_configuration, t.Name, t);
				}
			}

			// Remove any duplicates from the dictionary, because these create ambiguous matches. 
			// For example, "Foo.V1.ProductsController" and "Bar.V1.ProductsController" both map to "v1.products".
			foreach (string s in _duplicates)
			{
				dictionary.Remove(s);
			}
			return dictionary;
		}

		// Get a value from the route data, if present.
		static T GetRouteVariable<T>(IHttpRouteData routeData, string name)
		{
			object result;
			return routeData.Values.TryGetValue(name, out result) ? (T)result : default(T);
		}

		public HttpControllerDescriptor SelectController(HttpRequestMessage request)
		{
			IHttpRouteData routeData = request.GetRouteData();
			if (routeData == null)
			{
				throw new HttpResponseException(HttpStatusCode.NotFound);
			}

			// Get the namespace and controller variables from the route data.
			string namespaceName = GetRouteVariable<string>(routeData, NamespaceKey);
			if (namespaceName == null)
			{
				throw new HttpResponseException(HttpStatusCode.NotFound);
			}

			string controllerName = GetRouteVariable<string>(routeData, ControllerKey);
			if (controllerName == null)
			{
				throw new HttpResponseException(HttpStatusCode.NotFound);
			}

			// Find a matching controller.
			string key = String.Format(CultureInfo.InvariantCulture, "{0}.{1}", namespaceName, controllerName);

			HttpControllerDescriptor controllerDescriptor;
			if (_controllers.Value.TryGetValue(key, out controllerDescriptor))
			{
				return controllerDescriptor;
			}
			if (_duplicates.Contains(key))
			{
				throw new HttpResponseException(
					request.CreateErrorResponse(HttpStatusCode.InternalServerError,
						"Multiple controllers were found that match this request."));
			}
			throw new HttpResponseException(HttpStatusCode.NotFound);
		}

		public IDictionary<string, HttpControllerDescriptor> GetControllerMapping()
		{
			return _controllers.Value;
		}
	}
}