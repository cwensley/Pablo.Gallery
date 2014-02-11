using Pablo.Gallery.Models;
using System.Data.Entity;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System;
using System.Threading;

namespace Pablo.Gallery
{
	public class MvcApplication : HttpApplication
	{
		static MvcApplication()
		{
			// need to setup here otherwise it won't load embedded dll's properly
			Global.Initialize();
		}

		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();

			WebApiConfig.Register(GlobalConfiguration.Configuration);
			AuthConfig.RegisterAuth();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
			BundleConfig.RegisterExternalBundles(BundleTable.Bundles);
		}

		protected void Application_Error(object sender, EventArgs e)
		{
			Exception exception = Server.GetLastError();
			Console.WriteLine("Application error {0}", exception);
		}
	}
}