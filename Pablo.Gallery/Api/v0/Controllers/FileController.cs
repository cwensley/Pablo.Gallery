using Pablo.Gallery.Api.ApiModels;
using PabloDraw.Console;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace Pablo.Gallery.Api.v0.Controllers
{
	public class FileController : ApiController
	{
		Models.GalleryContext db = new Models.GalleryContext();

		[HttpGet]
		public IEnumerable<FileSummary> Index()
		{
			var files = from f in db.Files
					   select f;
			return from p in files.AsEnumerable() select new FileSummary { FileName = p.FileName };
		}

		[HttpGet]
		public FileDetail Index([FromUri(Name = "id")]string fileName)
		{
			return null;
		}

	}
}
