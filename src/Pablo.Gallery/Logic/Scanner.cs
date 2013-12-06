using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Pablo.Gallery.Models;
using System.Text;

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
						var packFileName = Path.Combine(Global.SixteenColorsArchiveLocation, packFileEntry);
						var extractor = Extractors.ExtractorFactory.GetInfoExtractor(packFileName);
						if (extractor == null)
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
						using (var db = new GalleryContext())
						{
							var pack = db.Packs.FirstOrDefault(r => r.FileName.ToLower() == packShortFile.ToLower());
							if (pack == null)
							{
								pack = new Pack
								{
									Name = Path.GetFileNameWithoutExtension(packFileEntry),
									FileName = packShortFile,
									Date = date
								};
								db.Packs.Add(pack);
								db.SaveChanges();
							}
							try
							{
								var archiveInfo = extractor.ExtractInfo(packFileName);
								//pack.ArchiveComment = archiveInfo.Comment;
								var files = archiveInfo.Files.ToArray();
								foreach (var fileInfo in files)
								{
									var fileInfo1 = fileInfo;
									try
									{
										ExtractFileInfo(db, pack, fileInfo, () => GetStream(packFileName, fileInfo1));
									}
									catch (Exception ex)
									{
										updateStatus(string.Format("Error extracting file '{0}', {1}", fileInfo.FileName, ex));
									}
								}
								var fileNames = files.Select(r=> Scanner.NormalizedPath(r.FileName).TrimStart('\\')).ToArray();
								foreach (var file in pack.Files.Where(r => !fileNames.Contains(r.FileName)))
								{
									db.Files.Remove(file);
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
									pack.Thumbnail = pack.Files.FirstOrDefault(r => Path.GetExtension(r.FileName).ToLowerInvariant() == ".nfo");
								if (pack.Thumbnail == null)
									pack.Thumbnail = pack.Files.FirstOrDefault(r => Path.GetFileNameWithoutExtension(r.FileName).ToLowerInvariant().Contains("info"));
								if (pack.Thumbnail == null)
									pack.Thumbnail = pack.Files.OrderBy(r => r.Order).FirstOrDefault(r => r.Type != null);
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

		Stream GetStream(string archiveFileName, Extractors.ExtractFileInfo fileInfo)
		{
			try
			{
				return fileInfo.GetStream();
			}
			catch
			{
				var extractor = Extractors.ExtractorFactory.GetFileExtractor(archiveFileName);
				var task = extractor.ExtractFile(archiveFileName, fileInfo.FileName);
				task.Wait();
				//if (task.Exception != null)
				//	throw task.Exception;
				return task.Result;
			}
		}

		Models.File ExtractFileInfo(GalleryContext db, Pack pack, Extractors.ExtractFileInfo fileInfo, Func<Stream> getStream)
		{
			var fileName = Scanner.NormalizedPath(fileInfo.FileName).TrimStart('\\');
			var file = pack.GetFileByName(fileName);
			var name = CanonicalName(fileName.Replace("\\", "/"));
			while (pack.Files.Any(r => r.Name == name && !ReferenceEquals(r, file)))
			{
				name += "_";
			}
			file.Name = name;
			file.Order = fileInfo.Order++;
			//updateStatus(string.Format("Processing file '{0}'", file.FileName));
			var format = FileFormat.FindByExtension(Path.GetExtension(file.NativeFileName));
			if (format != null)
			{
				file.Format = format.Name;
				file.Type = format.Type.Name;
			}
			else
			{
				file.Format = "ansi";
				file.Type = FileType.Character.Name;
			}

			if (file.Type == FileType.Character.Name)
			{
				if (false && file.Content == null)
				{
					using (var stream = getStream())
					{
						using (var outStream = new MemoryStream())
						{
							var parameters = new PabloDraw.ConvertParameters
							{
								InputStream = stream,
								InputFormat = file.Format,
								OutputFormat = "ascii",
								OutputStream = outStream
							};

							Global.PabloEngine.Convert(parameters);
							outStream.Position = 0;
							using (var reader = new StreamReader(outStream, Encoding.GetEncoding(437)))
							{
								var content = file.Content ?? (file.Content = new FileContent { File = file });
								content.Text = reader.ReadToEnd().Replace((char)0, ' ');
							}
						}
					}
				}
			}
			else if (file.Content != null)
				file.Content = null;

			return file;
		}

		static string CanonicalName(string name)
		{
			return Regex.Replace(name, @"(?![%]\d\d)([^a-zA-Z0-9/\-._~!$&'()*,;=:@])", "_", RegexOptions.Compiled); // excluded, but valid: +
		}
	}
}