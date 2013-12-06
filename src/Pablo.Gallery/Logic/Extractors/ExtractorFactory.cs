using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Pablo.Gallery.Logic.Extractors
{
	public static class ExtractorFactory
	{
		static readonly List<Extractor> extractors = new List<Extractor> {
			new UnzipExtractor(), // best at extracting files from zips
			new SevenZipExtractor(), // best at extracting all other files (rar/arj/7z/etc)
			new SharpZipLibExtractor(), // best at extracting zip info
			new SharpCompressExtractor(), // good at extracting rar/7z
			new DotNetZipExtractor(),
		};

		static readonly Extractor fallbackExtractor = new FallbackExtractor(extractors);

		public static IEnumerable<Extractor> Extractors { get { return extractors; } }

		public static Extractor GetFileExtractor(string fileName)
		{
			var extension = Path.GetExtension(fileName).ToLowerInvariant();
			if (fallbackExtractor.CanExtractFile(extension))
				return fallbackExtractor;
			return null;
			/*
			return extractors.FirstOrDefault(r => r.CanExtractFile(extension));
			*/
		}

		public static Extractor GetInfoExtractor(string fileName)
		{
			var extension = Path.GetExtension(fileName).ToLowerInvariant();
			if (fallbackExtractor.CanExtractInfo && fallbackExtractor.CanExtractFile(extension))
				return fallbackExtractor;
			return null;
			/*
			return extractors.FirstOrDefault(r => r.CanExtractInfo && r.CanExtractFile(extension));
			*/
		}

	}
}

