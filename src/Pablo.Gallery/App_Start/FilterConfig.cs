using System.Web;
using System.Web.Mvc;
using Pablo.Gallery.Logic.Filters;

namespace Pablo.Gallery
{
	public static class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new LoggingMvcExceptionFilter());
		}
	}
}