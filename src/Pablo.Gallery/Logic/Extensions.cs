using System;
using System.Data.Entity.Core.Objects;
using System.Data.Entity;
using System.Web.Mvc;
using System.IO;
using System.Security.Principal;
using System.Linq;

namespace Pablo.Gallery
{
	public static class Extensions
	{
		public static T WrapWebApiException<T>(this Controller controller, Func<T> action)
		{
			try
			{
				return action();
			}
			catch (System.Web.Http.HttpResponseException ex)
			{
				throw new System.Web.HttpException((int)ex.Response.StatusCode, null);
			}
		}

		public static Models.User CurrentUser(this IPrincipal principal)
		{
			using (var db = new Models.GalleryContext())
			{
				return db.Users.FirstOrDefault(r => r.UserName == principal.Identity.Name);
			}
		}
	}
}

