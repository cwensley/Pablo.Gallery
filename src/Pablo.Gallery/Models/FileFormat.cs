using System;
using System.Collections.Generic;
using System.Linq;

namespace Pablo.Gallery.Models
{
	public class FileFormat
	{
		public FileType Type { get; internal set; }
		public string Name { get; private set; }
		public string[] Extensions { get; private set; }

		public FileFormat(string format, params string[] extensions)
		{
			Name = format;
			Extensions = extensions;
		}

		static readonly object locker = new object();
		static Dictionary<string, FileFormat> extensionLookup;

		public static FileFormat FindByExtension(string extension)
		{
			if (extensionLookup == null)
				CreateExtensions();
			var ext = extension.TrimStart('.').ToLowerInvariant();
			FileFormat format;
			return extensionLookup.TryGetValue(ext, out format) ? format : null;
		}

		public static FileFormat Find(string name)
		{
			return FileType.Types.SelectMany(r => r.Formats).FirstOrDefault(r => string.Equals(r.Name, name, StringComparison.OrdinalIgnoreCase));
		}

		static void CreateExtensions()
		{
			lock (locker)
			{
				if (extensionLookup != null)
					return;
				extensionLookup = new Dictionary<string, FileFormat>();
				foreach (var item in FileType.Types)
				{
					foreach (var format in item.Formats)
					{
						foreach (var ext in format.Extensions)
						{
							try
							{
								extensionLookup.Add(ext, format);
							}
							catch (ArgumentException ex)
							{
								Console.WriteLine("could not add {0} to dictionary", ext);
							}
						}
					}
				}
			}
		}
	}
}

