using System;
using System.IO;
using System.Collections.Generic;
using Ionic.Zip;
using System.Threading.Tasks;
using System.Linq;

namespace Pablo.Gallery.Logic.Extractors
{
	public class DotNetZipExtractor : Extractor
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
					Comment = archive.Comment,
					Files = ExtractFiles(archive).ToArray()
				};
			}
		}

		static IEnumerable<ExtractFileInfo> ExtractFiles(ZipFile archive)
		{
			int order = 0;
			foreach (ZipEntry entry in archive)
			{
				if (entry.IsDirectory)
					continue;
				var currentEntry = entry;
				yield return new ExtractFileInfo
				{
					FileName = entry.FileName,
					Size = (int)entry.UncompressedSize,
					Order = order++,
					Comment = entry.Comment,
					GetStream = () =>
					{
						var stream = new MemoryStream();
						try
						{
							currentEntry.Extract(stream);
							stream.Position = 0;
							return stream;
						}
						catch (Exception ex)
						{
							//if (updateStatus != null)
							//	updateStatus(string.Format("Error extracting file '{0}': {1}", entry.FileName, ex));
							return null;
						}
					}
				};
			}
		}

		public override async Task<Stream> ExtractFile(string archiveFileName, string fileName)
		{
			var zf = new ZipFile(archiveFileName);
			var entry = zf[fileName];
			if (entry != null)
			{
				var stream = new MemoryStream((int)entry.UncompressedSize);
				entry.Extract(stream);
				stream.Position = 0;
				return stream;
			}
			return null;
		}
	}
}


