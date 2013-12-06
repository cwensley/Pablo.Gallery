using System;
using System.Web.Mvc;
using System.Net;
using System.Linq;

namespace Pablo.Gallery
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
	public class AuthorizeAttribute : System.Web.Mvc.AuthorizeAttribute
	{
		protected override void HandleUnauthorizedRequest(System.Web.Mvc.AuthorizationContext filterContext)
		{
			if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
			{
				// The user is not authenticated
				base.HandleUnauthorizedRequest(filterContext);
			}
			else if (!Roles.Split(',').Any(filterContext.HttpContext.User.IsInRole))
			{
				filterContext.Result = new ViewResult { ViewName = "~/Views/Shared/Unauthorized.cshtml" };
			}
			else
			{ 
				base.HandleUnauthorizedRequest(filterContext);
			}
		}
	}
}

