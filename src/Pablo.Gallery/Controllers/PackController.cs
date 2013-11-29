using System.Web.Mvc;

namespace Pablo.Gallery.Controllers
{
	public class PackController : Controller
	{
		readonly Api.V0.Controllers.PackController api = new Api.V0.Controllers.PackController();

		[OutputCache(Duration = 600)]
		public ActionResult Index()
		{
			return View(this.WrapWebApiException(() => api.Index()));
		}

		[OutputCache(Duration = 600)]
		public ActionResult Detail(string pack)
		{
			return View(this.WrapWebApiException(() => api.Index(pack)));
		}

		[OutputCache(Duration = 600)]
		public ActionResult File(string pack, string file)
		{
			return View(this.WrapWebApiException(() => api.Index(pack, file)));
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
				api.Dispose();
			base.Dispose(disposing);
		}
	}
}
