using System;
using System.IO;
using System.Threading.Tasks;

namespace Pablo.Gallery.Logic.Converters
{
	public class ConvertInfo
	{
		public Models.Pack Pack { get; set; }

		public Func<string, Task> ExtractFile { get; set; }

		public string FileName { get; set; }

		public string OutFileName { get; set; }

		public int? Zoom { get; set; }

		public int? MaxWidth { get; set; }
	}

	public abstract class Converter
	{
		public abstract Task<Stream> Convert(ConvertInfo info);
	}
}