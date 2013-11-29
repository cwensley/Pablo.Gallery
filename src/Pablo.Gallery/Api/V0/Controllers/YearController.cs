using Pablo.Gallery.Api.ApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace Pablo.Gallery.Api.V0.Controllers
{
	public class YearController : ApiController
	{
		readonly Models.GalleryContext db = new Models.GalleryContext();

		[HttpGet]
		public YearResult Index(int start = 0, int limit = 100)
		{
			var years = from p in db.Packs
			            group p by new { Year = p.Date.Value.Year } into g
						orderby g.Key.Year descending
			            select new YearSummary
			{
				Year = g.Key.Year,
				Packs = g.Count()
			};
			return new YearResult
			{
				Years = years.AsEnumerable().Skip(start).Take(limit)
			};
		}

		[HttpGet]
		public YearDetail Index([FromUri(Name = "id")]int year, int start = 0, int limit = 100)
		{
			var packs = from p in db.Packs
			            where p.Date.Value.Year == year
			            orderby p.Name
			            select p;

			return new YearDetail
			{
				Year = year,
				Packs = from p in packs.Skip(start).Take(limit).AsEnumerable()
				        select new PackSummary(p)
			};
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
				db.Dispose();
			base.Dispose(disposing);
		}
	}
}
