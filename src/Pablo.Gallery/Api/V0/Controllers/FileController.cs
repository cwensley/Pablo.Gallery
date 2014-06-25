using Pablo.Gallery.Api.ApiModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Data.Entity.Core.Metadata.Edm;

namespace Pablo.Gallery.Api.V0.Controllers
{
	public class FileController : ApiController
	{
		readonly Models.GalleryContext db = new Models.GalleryContext();

		[HttpGet]
		public IEnumerable<FileSummary> Index(string format = null, string type = null, string query = null, int page = 0, int pageSize = Global.DefaultPageSize)
		{
			var files = db.QueryFiles(format, type, query);
			return (from f in files.Skip(page * pageSize).Take(pageSize).AsEnumerable()
				select new FileSummary(f)).ToArray();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
				db.Dispose();
			base.Dispose(disposing);
		}
	}
}
