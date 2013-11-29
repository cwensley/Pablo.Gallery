using System;
using System.Web.Http.Filters;
using System.Net.Http;
using System.Net;
using System.Web;

namespace Pablo.Gallery.Logic
{
	public class LoggingApiExceptionFilter : ExceptionFilterAttribute
	{
		public override void OnException(HttpActionExecutedContext actionExecutedContext)
		{
			// TODO: logging
			Console.WriteLine("API error {0}\n{1}", actionExecutedContext.Request.RequestUri, actionExecutedContext.Exception);
			actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
		}
	}

	public class LoggingMvcExceptionFilter : System.Web.Mvc.HandleErrorAttribute
	{
		public override void OnException(System.Web.Mvc.ExceptionContext filterContext)
		{
			// TODO: logging
			Console.WriteLine("Site error {0}\n{1}", filterContext.HttpContext.Request.Url, filterContext.Exception);
			base.OnException(filterContext);
		}
	}
}

