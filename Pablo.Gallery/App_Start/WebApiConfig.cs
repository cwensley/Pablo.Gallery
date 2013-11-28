using Pablo.Gallery.Logic;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Dispatcher;

namespace Pablo.Gallery
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			config.Filters.Add(new LoggingApiExceptionFilter());

			config.Routes.MapHttpRoute(
				name: "VersionedApi",
				routeTemplate: "api/{version}/{controller}/{id}/{*id2}",
				defaults: new { version = "v0", id = RouteParameter.Optional, id2 = RouteParameter.Optional }
			);

			config.Services.Replace(typeof(IHttpControllerSelector), new NamespaceHttpControllerSelector(config, 1));

			config.Formatters.JsonFormatter.AddQueryStringMapping("type", "json", "application/json");

			config.Formatters.XmlFormatter.AddQueryStringMapping("type", "xml", "application/xml");
		}
	}
}
