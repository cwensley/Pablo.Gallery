using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Migrations;

namespace Pablo.Gallery.Controllers
{
    public class HomeController : Controller
    {
		Models.GalleryContext db = new Models.GalleryContext();

        public ActionResult Index()
        {
			/*
			db.Packs.AddOrUpdate(r => r.Name,
				new Models.Pack { Name = "Hello", Date = DateTime.Now },
				new Models.Pack { Name = "There", Date = DateTime.Now },
				new Models.Pack { Name = "My", Date = DateTime.Now },
				new Models.Pack { Name = "Friend", Date = DateTime.Now }
			);
			db.SaveChanges();*/

			var packs = from p in db.Packs select p;
			ViewBag.Packs = packs;

            return View();
        }

		public ActionResult Scan()
		{
			var scanner = new Logic.Scanner();
			scanner.ScanPacks(db);
			return View();
		}
    }
}
