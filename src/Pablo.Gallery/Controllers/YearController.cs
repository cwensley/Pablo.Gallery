using System.Web.Mvc;
using Newtonsoft.Json;
using System.Linq;

namespace Pablo.Gallery.Controllers
{
	public class YearController : Controller
	{
		public ActionResult Detail(int year)
		{
			return View(year);
		}

		public ActionResult Index()
		{
			return View();
		}
	}
}
