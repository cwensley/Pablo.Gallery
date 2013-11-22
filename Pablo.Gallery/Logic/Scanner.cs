using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Pablo.Gallery.Logic
{
	public class Scanner
	{
		public void ScanPacks(Models.GalleryContext db)
		{
			var baseDir = @"Z:\Projects\External\sixteencolors-archive";
			var extractor = new Extractor();
			foreach (var dir in Directory.EnumerateDirectories(baseDir))
			{
				var idx = dir.LastIndexOf(Path.DirectorySeparatorChar);
				if (idx < 0)
					continue;
				var yearString = dir.Substring(idx + 1);
				int year;
				if (int.TryParse(yearString, out year))
				{
					foreach (var packFile in Directory.EnumerateFiles(dir))
					{
						DateTime? date = null;
						var match = Regex.Match(packFile, @"^(.+?)(?<month>\d\d)(?<year>\d\d)[.](\w+)$", RegexOptions.ExplicitCapture);
						if (match.Success)
						{
							var monthString = match.Groups["month"].Value;
							int month;
							if (int.TryParse(monthString, out month) && month >= 1 && month <= 12)
							{
								date = new DateTime(year, month, 1);
							}
							else
								date = new DateTime(year, 1, 1);
						}
						else
							date = new DateTime(year, 1, 1);
						
						var packShortFile = packFile.Substring(baseDir.Length);
						var packShortFileF = packShortFile.Replace("\\", "\\\\");
						var packs = from p in db.Packs where p.FileName.EndsWith(packShortFileF) select p;
						var pack = packs.FirstOrDefault();
						if (pack == null)
						{
							pack = new Models.Pack
							{
								Name = Path.GetFileNameWithoutExtension(packFile),
								FileName = packShortFile,
								Date = date
							};
							db.Packs.Add(pack);
						}
						else
						{
							if (pack.FileName.StartsWith(baseDir))
							{
								pack.FileName = pack.FileName.Substring(baseDir.Length);
							}
						}
						/*foreach (var file in extractor.ExtractAll(pack))
						{
							db.Files.Add(file);
						}*/
						db.SaveChanges();

					}
				}
			}
		}
	}
}