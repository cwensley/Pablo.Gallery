using System.Web.Mvc;

namespace Pablo.Gallery.Controllers
{
	public class YearController : Controller
	{
		readonly Api.V0.Controllers.YearController api = new Api.V0.Controllers.YearController();

		public ActionResult Detail(int year)
		{
			return View(this.WrapWebApiException(() => api.Index(year: year)));
		}

		public ActionResult Index()
		{
			return View(this.WrapWebApiException(() => api.Index()));
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
				api.Dispose();
			base.Dispose(disposing);
		}
	}
}
