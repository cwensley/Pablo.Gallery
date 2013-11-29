using System;
using System.Threading.Tasks;
using System.IO;

namespace Pablo.Gallery.Logic.Converters
{
	public abstract class FileCacheConverter : Converter
	{
		public abstract Task ConvertFile(ConvertInfo info, string inFile, string outFile);

		public override async Task<Stream> Convert(ConvertInfo info)
		{
			var packDir = Path.Combine(Global.SixteenColorsCacheLocation, info.Pack.Name);

			var outFileName = Path.Combine(packDir, info.OutFileName);
			if (!File.Exists(outFileName))
			{
				// save raw input file to cache
				var inFileName = Path.Combine(packDir, info.FileName);
				Directory.CreateDirectory(Path.GetDirectoryName(inFileName));
				if (!File.Exists(inFileName))
				{
					await info.ExtractFile(inFileName);

					#if DEBUG
					if (!File.Exists(inFileName))
					{
						throw new FileNotFoundException("File was not extracted", inFileName);
					}
					#endif
				}

				await ConvertFile(info, inFileName, outFileName);
				if (File.Exists(outFileName))
				{
					return File.OpenRead(outFileName);
				}
			}
			else
			{
				return File.OpenRead(outFileName);
			}
			return null;
		}
	}
}

