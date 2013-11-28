using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Pablo.Gallery.Logic
{
	public class Scanner
	{
		public static string NormalizedPath(string path)
		{
			return path.Replace(@"/", @"\");
		}

		public static string NativePath(string path)
		{
			return path.Replace(@"\", Path.DirectorySeparatorChar.ToString());
		}

		public void ScanPacks(Action<string> updateStatus)
		{
			var startTime = DateTime.Now;
			updateStatus(string.Format("Scanning began {0:g}", startTime));

			var dirs = Directory.EnumerateDirectories(Global.SixteenColorsArchiveLocation);
			//dirs = dirs.SkipWhile(r => !r.EndsWith("1996", StringComparison.InvariantCultureIgnoreCase));
			//dirs = dirs.Where(r => r.EndsWith("1997", StringComparison.OrdinalIgnoreCase));
			foreach (var dir in dirs)
			{
				var idx = dir.LastIndexOf(Path.DirectorySeparatorChar);
				if (idx < 0)
					continue;
				var yearString = dir.Substring(idx + 1);
				int year;
				if (int.TryParse(yearString, out year))
				{
					var packNames = Directory.EnumerateFiles(dir);
					//packNames = packNames.SkipWhile(r => !Path.GetFileName(r).StartsWith("blde9612", StringComparison.InvariantCultureIgnoreCase));
					foreach (var packFileEntry in packNames)
					{
						var packFile = NormalizedPath(packFileEntry);
						var ext = Path.GetExtension(packFile);
						if (!new[] { ".zip", ".rar", ".7z" }.Contains(ext, StringComparer.OrdinalIgnoreCase))
							continue;
						DateTime? date;
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


						var packShortFile = packFile.Substring(Global.SixteenColorsArchiveLocation.Length).TrimStart('\\');
						updateStatus(string.Format("Updating pack {0}", packShortFile));
						using (var db = new Models.GalleryContext())
						{
							var packShortFileF = packShortFile;
							if (db.IsPostgres)
								packShortFileF.Replace(@"\", @"\\"); // hack, npgsql doesn't escape strings for contains

							var packs = from p in db.Packs
							            where p.FileName.EndsWith(packShortFileF)
							            select p;
							var pack = packs.FirstOrDefault();
							if (pack == null)
							{
								pack = new Models.Pack
								{
									Name = Path.GetFileNameWithoutExtension(packFileEntry),
									FileName = packShortFile,
									Date = date
								};
								db.Packs.Add(pack);
								db.SaveChanges();
							}
							else
							{
								/*
								string winBaseDir = @"Z:\Projects\External\sixteencolors-archive";
								if (pack.FileName.StartsWith(winBaseDir, StringComparison.Ordinal))
								{
									pack.FileName = pack.FileName.Substring(winBaseDir.Length).TrimStart('\\');
								}
								else*/ if (pack.FileName.StartsWith(@"\", StringComparison.Ordinal))
									pack.FileName = pack.FileName.TrimStart('\\');
							}
							try
							{
								var packFileName = Path.Combine(Global.SixteenColorsArchiveLocation, pack.NativeFileName);
								var extractor = Extractors.ExtractorFactory.GetInfoExtractor(packFileName);
								var archiveInfo = extractor.ExtractInfo(packFileName);
								//pack.ArchiveComment = archiveInfo.Comment;
								foreach (var fileInfo in archiveInfo.Files)
								{
									ExtractFileInfo(db, pack, fileInfo);
								}
							}
							catch (Exception ex)
							{
								updateStatus(string.Format("Error extracting pack '{0}', {1}", pack.FileName, ex));
							}

							if (/*pack.Thumbnail == null &&*/ pack.Files != null)
							{
								pack.Thumbnail = pack.Files.FirstOrDefault(r => r.FileName.ToLowerInvariant() == "file_id.diz");
								if (pack.Thumbnail == null)
									pack.Thumbnail = pack.Files.FirstOrDefault(r => Path.GetExtension(r.FileName).ToLowerInvariant() == ".diz");
								if (pack.Thumbnail == null)
									pack.Thumbnail = pack.Files.FirstOrDefault(r => r.Type != null);
							}
							db.SaveChanges();
						}
					}
				}
			}
			var endTime = DateTime.Now;
			var elapsed = endTime - startTime;
			updateStatus(string.Format("Scanning ended {0:g} ({1:hh\\:mm\\:ss})", endTime, elapsed));
		}

		Models.File ExtractFileInfo(Models.GalleryContext db, Models.Pack pack, Extractors.ExtractFileInfo fileInfo)
		{
			var fileName = Scanner.NormalizedPath(fileInfo.FileName).TrimStart('\\');
			var file = pack.GetFileByName(fileName);

			file.Order = fileInfo.Order++;
			//updateStatus(string.Format("Processing file '{0}'", file.FileName));
			if (string.IsNullOrEmpty(file.Format))
				file.Format = Global.PabloEngine.DetectFormat(file.FileName);
			if (string.IsNullOrEmpty(file.Type))
				file.Type = Global.PabloEngine.DetectType(file.FileName);

			return file;
		}
	}
}