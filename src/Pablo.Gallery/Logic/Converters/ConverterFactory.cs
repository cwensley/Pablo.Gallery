using System;

namespace Pablo.Gallery.Logic.Converters
{
	public static class ConverterFactory
	{
		static readonly Converter inlineConverter = new PabloInlineConverter();
		static readonly Converter consoleConverter = new PabloConsoleConverter();

		public static Converter GetConverter(string fileName)
		{
			//return consoleConverter;
			return inlineConverter;
		}
	}
}

