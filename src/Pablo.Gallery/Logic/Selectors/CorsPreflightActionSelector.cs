using System;
using System.Web.Http.Controllers;
using System.Net.Http;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Collections.ObjectModel;
using System.Web.Http.Filters;
using System.Threading.Tasks;
using System.Threading;
using Pablo.Gallery.Logic.Filters;
using System.Collections.Concurrent;

namespace Pablo.Gallery.Logic.Selectors
{
	public class CorsPreflightActionSelector : ApiControllerActionSelector
	{
		const string Origin = "Origin";
		const string AccessControlRequestMethod = "Access-Control-Request-Method";
		const string AccessControlRequestHeaders = "Access-Control-Request-Headers";
		const string AccessControlAllowMethods = "Access-Control-Allow-Methods";
		const string AccessControlAllowHeaders = "Access-Control-Allow-Headers";

		public override HttpActionDescriptor SelectAction(HttpControllerContext controllerContext)
		{
			HttpRequestMessage originalRequest = controllerContext.Request;
			bool isCorsRequest = originalRequest.Headers.Contains(Origin);
			if (originalRequest.Method == HttpMethod.Options && isCorsRequest)
			{
				string accessControlRequestMethod = originalRequest.Headers.GetValues(AccessControlRequestMethod).FirstOrDefault();
				if (!string.IsNullOrEmpty(accessControlRequestMethod))
				{
					var modifiedRequest = new HttpRequestMessage(new HttpMethod(accessControlRequestMethod), originalRequest.RequestUri);
					controllerContext.Request = modifiedRequest;
					var actionDescriptor = base.SelectAction(controllerContext);
					controllerContext.Request = originalRequest;
					if (actionDescriptor != null)
					{
						var allow = actionDescriptor.ControllerDescriptor.GetCustomAttributes<EnableCorsAttribute>().Any();
						allow |= actionDescriptor.GetCustomAttributes<EnableCorsAttribute>().Any();
						if (allow)
						{
							return new PreflightActionDescriptor(actionDescriptor, accessControlRequestMethod);
						}
					}
				}
			}

			return base.SelectAction(controllerContext);
		}

		class PreflightActionDescriptor : HttpActionDescriptor
		{
			readonly HttpActionDescriptor original;
			readonly string accessControlRequestMethod;

			public PreflightActionDescriptor(HttpActionDescriptor originalAction, string accessControlRequestMethod)
			{
				this.original = originalAction;
				this.accessControlRequestMethod = accessControlRequestMethod;
			}

			public override string ActionName
			{
				get { return this.original.ActionName; }
			}

			public override HttpActionBinding ActionBinding
			{
				get { return original.ActionBinding; }
				set { original.ActionBinding = value; }
			}

			public override Collection<IFilter> GetFilters()
			{
				return original.GetFilters();
			}

			public override Collection<T> GetCustomAttributes<T>()
			{
				return original.GetCustomAttributes<T>();
			}

			public override ConcurrentDictionary<object, object> Properties
			{
				get { return original.Properties; }
			}

			public override Collection<FilterInfo> GetFilterPipeline()
			{
				return original.GetFilterPipeline();
			}

			public override Collection<HttpMethod> SupportedHttpMethods
			{
				get { return original.SupportedHttpMethods; }
			}

			public override IActionResultConverter ResultConverter
			{
				get { return original.ResultConverter; }
			}

			public override Collection<HttpParameterDescriptor> GetParameters()
			{
				return original.GetParameters();
			}

			public override Type ReturnType
			{
				get { return typeof(HttpResponseMessage); }
			}

			public override async Task<object> ExecuteAsync(HttpControllerContext controllerContext, IDictionary<string, object> arguments, CancellationToken cancellationToken)
			{
				var response = new HttpResponseMessage(HttpStatusCode.OK);

				// No need to add the Origin; this will be added by the action filter
				response.Headers.Add(AccessControlAllowMethods, accessControlRequestMethod);

				string requestedHeaders = string.Join(", ", controllerContext.Request.Headers.GetValues(AccessControlRequestHeaders));

				if (!string.IsNullOrEmpty(requestedHeaders))
				{
					response.Headers.Add(AccessControlAllowHeaders, requestedHeaders);
				}

				return response;
			}
		}
	}
}

