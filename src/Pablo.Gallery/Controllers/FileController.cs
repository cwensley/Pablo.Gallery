using System.Web.Mvc;
using System.Collections.Generic;
using Pablo.Gallery.Api.ApiModels;
using Newtonsoft.Json;
using System.Linq;
using System.IO;
using System.Threading;

namespace Pablo.Gallery.Controllers
{
	public class FileController : Controller
	{
		public ActionResult Index(string format = null, string type = null, string query = null)
		{
			ViewBag.Params = JsonConvert.SerializeObject(new {
				Format = format,
				Type = type,
				Query = query
			});
			return View();
		}
	}
}
