using System;
using System.IO;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip;
using System.Threading.Tasks;
using System.Linq;

namespace Pablo.Gallery.Logic.Extractors
{
	public class SharpZipLibExtractor : Extractor
	{
		public override bool CanExtractInfo { get { return true; } }

		public override bool CanExtractFile(string extension)
		{
			return extension == ".zip";
		}

		public override ExtractArchiveInfo ExtractInfo(string archiveFileName)
		{
			using (var archive = new ZipFile(archiveFileName))
			{
				var fileInfo = new FileInfo(archiveFileName);
				return new ExtractArchiveInfo
				{
					Size = (int)fileInfo.Length,
					Comment = archive.ZipFileComment,
					Files = ExtractFiles(archive).ToArray()
				};
			}
		}

		static IEnumerable<ExtractFileInfo> ExtractFiles(ZipFile archive)
		{
			int order = 0;
			foreach (ZipEntry entry in archive)
			{
				if (!entry.IsFile || entry.IsDirectory)
					continue;
				var currentEntry = entry;
				var currentArchive = archive;
				yield return new ExtractFileInfo
				{
					FileName = entry.Name,
					Size = (int)entry.Size,
					Order = order++,
					Comment = entry.Comment,
					GetStream = () => currentArchive.GetInputStream(currentEntry)
				};
			}
		}

		public override async Task<Stream> ExtractFile(string archiveFileName, string fileName)
		{
			var archive = new ZipFile(archiveFileName);
			var entry = archive.GetEntry(fileName.Replace('\\', '/'));
			return entry != null ? archive.GetInputStream(entry) : null;
		}
	}
}


