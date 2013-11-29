using System;
using System.IO;
using System.Collections.Generic;
using Ionic.Zip;
using System.Diagnostics;
using System.Configuration;
using System.Threading.Tasks;

namespace Pablo.Gallery.Logic.Extractors
{
	public class SevenZipExtractor : CommandExtractor
	{
		readonly string exePath;

		protected override string ExecutablePath { get { return exePath; } }

		public SevenZipExtractor(string exePath = null)
		{
			this.exePath = exePath ?? ConfigurationManager.AppSettings["7zipPath"] ?? "/usr/bin/7z";
		}

		public override bool CanExtractFile(string extension)
		{
			return extension == ".zip" || extension == ".rar" || extension == ".7z" || extension == ".arj";
		}

		protected override string GetStreamArgs(string archiveFileName, string fileName)
		{
			return string.Format("x \"{0}\" -so \"{1}\"", archiveFileName, fileName.Replace("\\", "/"));
		}

		protected override string GetDirectArgs(string archiveFileName, string fileName, string destinationDir)
		{
			return string.Format("e \"{0}\" \"-o{1}\" \"{2}\"", archiveFileName, destinationDir, fileName.Replace("\\", "/"));
		}
	}
}


