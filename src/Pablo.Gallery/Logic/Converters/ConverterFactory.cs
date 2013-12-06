using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Pablo.Gallery.Logic.Converters
{
	public static class ConverterFactory
	{
		static readonly List<Converter> converters = new List<Converter> {
			new ImageConverter(),
			new PabloInlineConverter(),
			//new PabloConsoleConverter(),
			new ModConverter(),
			new AudioConverter(),
			new MidiConverter(),
		};
		public static Converter GetConverter(ConvertInfo info)
		{
			return converters.FirstOrDefault(r => r.CanConvert(info));
		}
	}
}

