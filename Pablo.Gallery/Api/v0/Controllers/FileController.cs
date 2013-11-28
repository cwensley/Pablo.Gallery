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

namespace Pablo.Gallery.Api.V0.Controllers
{
	public class FileController : ApiController
	{
		readonly Models.GalleryContext db = new Models.GalleryContext();

		[HttpGet]
		public IEnumerable<FileSummary> Index(string format = null, string type = null, string query = null, int start = 0, int limit = 100)
		{
			var files = from f in db.Files
			            where
			                (format == null || f.Format.ToLower() == format.ToLower())
			                && (type == null || f.Type.ToLower() == type.ToLower())
			                && (query == null || f.FileName.ToLower().Contains(query.ToLower()))
			            orderby f.Pack.Date descending, f.FileName
			            select f;
			return from f in files.Skip(start).Take(limit).AsEnumerable()
			       select new FileSummary(f);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
				db.Dispose();
		}
	}
}
