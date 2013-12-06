using System;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;

namespace Pablo.Gallery.Controllers
{
	[Authorize(Roles = "admin")]
	[HandleError(ExceptionType = typeof(UnauthorizedAccessException), View = "Pack")]
	public class ScannerController : Controller
	{
		public ActionResult Index()
		{
			ViewBag.Started = process != null && !process.Done;
			return View();
		}

		public class ProcessInfo
		{
			[JsonIgnore]
			public Task Task { get; set; }

			public bool Abort { get; set; }
			public bool Done { get; set; }
			public string Message { get; set; }
		}

		static ProcessInfo process;

		[HttpPost]
		public ActionResult Start()
		{
			if (process == null)
			{
				process = new ProcessInfo();
				process.Task = Task.Factory.StartNew(() => RunTask(process));
				return Content("started");
			}
			return Content("already started");
		}

		[HttpPost]
		public ActionResult Progress()
		{
			return Json(process);
		}

		[HttpPost]
		public ActionResult End()
		{
			if (process != null)
			{
				if (!process.Task.IsCompleted)
				{
					process.Abort = true;
					Task.WaitAll(process.Task);
				}
				var ret = Json(process);
				process = null;
				return ret;
			}
			return Content("Not running");
		}

		static void RunTask(ProcessInfo process)
		{
			var sb = new StringBuilder();
			try
			{
				var scanner = new Logic.Scanner();
				scanner.ScanPacks(status => {
					sb.AppendLine(status);
					process.Message = sb.ToString();
				});
				process.Done = true;
			}
			catch (Exception ex)
			{
				process.Done = true;
				sb.AppendLine("Error: " + ex.ToString());
				process.Message = sb.ToString();
			}
		}
	}
}
