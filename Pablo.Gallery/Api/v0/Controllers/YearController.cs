using Pablo.Gallery.Api.ApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace Pablo.Gallery.Api.v0.Controllers
{
	public class YearController : ApiController
	{
		Models.GalleryContext db = new Models.GalleryContext();

		[HttpGet]
		public YearResult Index()
		{
			return new YearResult
			{
				Years = from p in db.Packs
						group p by new { Year = p.Date.Value.Year } into g
						select new YearSummary
						{
							Year = g.Key.Year,
							Packs = g.Count()
						}
			};
		}

		[HttpGet]
		public YearDetail Index([FromUri(Name = "id")]int year)
		{
			var packs = from p in db.Packs
						where p.Date.Value.Year == year
						select p;

			return new YearDetail
			{
				Year = year,
				Packs = from p in packs.AsEnumerable() select PackSummary.FromModel(p)
			};
		}
	}
}
