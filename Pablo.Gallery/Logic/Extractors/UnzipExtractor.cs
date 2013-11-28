using System;
using System.IO;
using System.Collections.Generic;
using Ionic.Zip;
using System.Diagnostics;
using System.Configuration;
using System.Threading.Tasks;

namespace Pablo.Gallery.Logic.Extractors
{
	public class UnzipExtractor : CommandExtractor
	{
		readonly string exePath;

		protected override string ExecutablePath { get { return exePath; } }

		public UnzipExtractor(string exePath = null)
		{
			this.exePath = exePath ?? ConfigurationManager.AppSettings["UnzipPath"] ?? "/usr/bin/unzip";
		}

		public override bool CanExtractFile(string extension)
		{
			return extension == ".zip";
		}

		protected override string GetStreamArgs(string archiveFileName, string fileName)
		{
			return string.Format("-p \"{0}\" \"{1}\"", archiveFileName, fileName.Replace("\\", "/"));
		}

		protected override string GetDirectArgs(string archiveFileName, string fileName, string destinationDir)
		{
			return string.Format("\"{0}\" \"{2}\" -d \"{1}\" ", archiveFileName, destinationDir, fileName.Replace("\\", "/"));
		}
	}
}


