using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Pablo.Gallery.Logic.Extractors
{
	public class ExtractFileInfo
	{
		public string FileName { get; set; }
		public int Size { get; set; }
		public int Order { get; set; }
		public string Comment { get; set; }
		public Func<Stream> GetStream { get; set; }
	}

	public class ExtractArchiveInfo
	{
		public IEnumerable<ExtractFileInfo> Files { get; set; }
		public string Comment { get; set; }
		public int Size { get; set; }
	}

	public abstract class Extractor
	{
		public abstract bool CanExtractInfo { get; }

		public abstract bool CanExtractFile(string extension);

		public abstract Task<Stream> ExtractFile(string archiveFileName, string fileName);

		public virtual async Task ExtractFile(string archiveFileName, string fileName, string destinationFileName)
		{
			var stream = await ExtractFile(archiveFileName, fileName);
			if (stream != null)
			{
				using (var fileStream = File.Create(destinationFileName))
				{
					stream.CopyTo(fileStream);
				}
			}
			else
				throw new Exception(string.Format("Error extracting file '{0}' from archive '{1}'", fileName, archiveFileName));
		}

		public abstract ExtractArchiveInfo ExtractInfo(string archiveFileName);
	}
}