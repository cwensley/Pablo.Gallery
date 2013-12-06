﻿using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Text;
using System.Configuration;

namespace Pablo.Gallery.Logic.Converters
{
	class ImageConverter : FileCacheConverter
	{
		readonly string convertPath;

		public ImageConverter(string convertPath = null)
		{
			this.convertPath = convertPath ?? ConfigurationManager.AppSettings["ImageMagickPath"];
		}

		public override bool CanConvert(ConvertInfo info)
		{
			return info.InputType == Models.FileType.Image.Name;
		}

		public override Task ConvertFile(ConvertInfo info, string inFile, string outFile)
		{
			var tcs = new TaskCompletionSource<object>();
			var args = new StringBuilder();
			args.AppendFormat("\"{0}\" -alpha on -background none -flatten ", inFile);

			if (info.MaxWidth != null)
				args.AppendFormat(" -resize '{0}>'", info.MaxWidth.Value);

			args.AppendFormat(" \"{0}\"", outFile);

			var process = new Process
			{
				EnableRaisingEvents = true,
				StartInfo = new ProcessStartInfo
				{
					FileName = convertPath,
					Arguments = args.ToString(),
					UseShellExecute = false,
					RedirectStandardOutput = true, // turn these off (with mono) to see output errors
					RedirectStandardError = false,
					CreateNoWindow = true
				}
			};

			process.Exited += (s, e) =>
			{
				if (process.ExitCode == 0)
					tcs.SetResult(true);
				else
					tcs.SetException(new Exception(process.StandardOutput.ReadToEnd()));
			};
			process.Start();
			return tcs.Task;
		}
	}
}