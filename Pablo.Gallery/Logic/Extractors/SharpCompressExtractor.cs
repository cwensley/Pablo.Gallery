using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pablo.Gallery.Logic.Extractors
{
	public class SharpCompressExtractor : Extractor
	{
		public override bool CanExtractInfo { get { return true; } }

		public override bool CanExtractFile(string extension)
		{
			return extension == ".zip" || extension == ".7z" || extension == ".rar" || extension == ".tar.gz";
		}

		public override ExtractArchiveInfo ExtractInfo(string archiveFileName)
		{
			using (var archive = SharpCompress.Archive.ArchiveFactory.Open(archiveFileName))
			{
				return new ExtractArchiveInfo
				{
					Size = (int)archive.TotalSize,
					//Comment = archive.Comment,
					Files = ExtractFiles(archive).ToArray()
				};
			}
		}

		IEnumerable<ExtractFileInfo> ExtractFiles(SharpCompress.Archive.IArchive archive)
		{
			int order = 0;
			foreach (var entry in archive.Entries)
			{
				if (entry.IsDirectory)
					continue;
				var currentEntry = entry;
				yield return new ExtractFileInfo
				{
					FileName = entry.FilePath.Replace('/', '\\'),
					Size = (int)entry.Size,
					Order = order++,
					GetStream = currentEntry.OpenEntryStream
				};
			}
		}

		public override async Task<Stream> ExtractFile(string archiveFileName, string fileName)
		{
			fileName = fileName.Replace('\\', '/'); // always in unix form
			archiveFileName = Path.Combine(Global.SixteenColorsArchiveLocation, archiveFileName);
			using (var archive = SharpCompress.Archive.ArchiveFactory.Open(archiveFileName))
			{
				var entry = archive.Entries.FirstOrDefault(r => string.Equals(r.FilePath, fileName, StringComparison.OrdinalIgnoreCase));
				if (entry != null)
				{
					var ms = new MemoryStream((int)entry.Size);
					entry.WriteTo(ms);
					ms.Position = 0;
					return ms;
				}
			}
			return null;
		}
	}
}


