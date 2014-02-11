using Pablo.Gallery.Api.ApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using Pablo.Gallery.Logic.Filters;

namespace Pablo.Gallery.Api.V0.Controllers
{
	public class YearController : ApiController
	{
		readonly Models.GalleryContext db = new Models.GalleryContext();

		[HttpGet, EnableCors]
		public YearResult Index(int page = 0, int size = Global.DefaultPageSize)
		{
			var years = from p in db.Packs
			            group p by new { Year = p.Date.Value.Year } into g
						orderby g.Key.Year descending
			            select g;
			var results = years.AsEnumerable().Skip(page * size).Take(size);
			return new YearResult
			{
				Years = from y in results select new YearSummary(y.Key.Year, y.Count())
			};
		}

		[HttpGet, EnableCors]
		public YearDetail Index([FromUri(Name = "id")]int year, int page = 0, int size = Global.DefaultPageSize)
		{
			var packs = from p in db.Packs
			            where p.Date.Value.Year == year
			            orderby p.Name
			            select p;

			var results = packs.Skip(page * size).Take(size).AsEnumerable();
			return new YearDetail
			{
				Year = year,
				Packs = from p in results select new PackSummary(p)
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
