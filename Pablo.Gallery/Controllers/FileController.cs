using System.Web.Mvc;

namespace Pablo.Gallery.Controllers
{
	public class FileController : Controller
	{
		readonly Api.V0.Controllers.FileController api = new Api.V0.Controllers.FileController();

		[OutputCache(Duration = 600)]
		public ActionResult Index(string format = null, string type = null, string query = null)
		{
			return View(this.WrapWebApiException(() => api.Index(format, type, query)));
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
				api.Dispose();
			base.Dispose(disposing);
		}
	}
}
