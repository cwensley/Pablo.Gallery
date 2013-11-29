using System;
using System.Data.Entity.Core.Objects;
using System.Data.Entity;
using System.Web.Mvc;
using System.IO;

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
	}
}

