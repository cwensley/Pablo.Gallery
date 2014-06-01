using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using Pablo.Gallery.Logic;
using Pablo.Gallery.Logic.Filters;
using System.Web.Http.Controllers;
using Pablo.Gallery.Logic.Selectors;

namespace Pablo.Gallery
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{

			config.Filters.Add(new LoggingApiExceptionFilter());

			config.Routes.MapHttpRoute(
				name: "api",
				routeTemplate: "api/{version}/{controller}/{id}",
				defaults: new { version = "v0", id = RouteParameter.Optional }
			);

			// need this separate as Url.RouteUrl does not work with a wildcard in the route
			// this is used to allow files in subfolders of a pack to be retrieved.
			config.Routes.MapHttpRoute(
				name: "apipath",
				routeTemplate: "api/{version}/{controller}/{id}/{*path}",
				defaults: new { version = "v0", id = RouteParameter.Optional, path = RouteParameter.Optional }
			);

			config.Services.Replace(typeof(IHttpControllerSelector), new NamespaceHttpControllerSelector(config, 1));

			config.Formatters.JsonFormatter.AddQueryStringMapping("type", "json", "application/json");

			config.Formatters.XmlFormatter.AddQueryStringMapping("type", "xml", "application/xml");

			//config.MessageHandlers.Add(new CorsHandler());
			config.Services.Replace(typeof(IHttpActionSelector), new CorsPreflightActionSelector());
		}
	}
}
