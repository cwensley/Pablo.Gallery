using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Pablo.Gallery.Logic
{
	public class Extractor
	{
		//var baseDir = "/Users/curtis/Projects/External/sixteencolors-archive";
		string baseDir = @"Z:\Projects\External\sixteencolors-archive";
		//var baseDir = "/Users/curtis/Downloads/pablo";
		//var baseDir = @"Z:\Downloads\pablo";


		public Stream Extract(string pack, string file)
		{
			var packFileName = Path.Combine(baseDir, pack);
			using (var archive = SharpCompress.Archive.ArchiveFactory.Open(packFileName, SharpCompress.Common.Options.KeepStreamsOpen))
			{
				var entry = archive.Entries.FirstOrDefault(r => string.Equals(r.FilePath, file, StringComparison.OrdinalIgnoreCase));
				if (entry != null)
				{
					return entry.OpenEntryStream();
				}
			}
			return null;
		}

		public IEnumerable<Models.File> ExtractAll(Models.Pack pack)
		{
			var packFileName = Path.Combine(baseDir, pack.FileName);
			using (var archive = SharpCompress.Archive.ArchiveFactory.Open(packFileName, SharpCompress.Common.Options.KeepStreamsOpen))
			{
				foreach (var entry in archive.Entries)
				{
					var file = new Models.File
					{
						Name = Path.GetFileNameWithoutExtension(entry.FilePath),
						FileName = entry.FilePath,
						Pack = pack
					};

					yield return file;
				}
			}
		}
	}
}