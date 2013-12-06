using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Text;
using System.Configuration;

namespace Pablo.Gallery.Logic.Converters
{
	class PabloConsoleConverter : FileCacheConverter
	{
		readonly string monoPath;

		public PabloConsoleConverter(string monoPath = null)
		{
			this.monoPath = monoPath ?? ConfigurationManager.AppSettings["MonoPath"];
		}

		public override bool CanConvert(ConvertInfo info)
		{
			return info.InputType == Models.FileType.Character.Name || info.InputType == Models.FileType.Image.Name || info.InputType == Models.FileType.Rip.Name;
		}

		public override Task ConvertFile(ConvertInfo info, string inFile, string outFile)
		{
			var tcs = new TaskCompletionSource<object>();
			var convertExe = "PabloDraw.Console.exe";
			var appPath = HttpContext.Current.Request.MapPath("~/Util");
			convertExe = Path.Combine(appPath, convertExe);
			var args = new StringBuilder();
			args.AppendFormat(" --convert \"{0}\" --out \"{1}\"", inFile, outFile);
			if (info.Zoom != null)
				args.AppendFormat(" --zoom {0:0.00}", info.Zoom.Value / 100f);
			if (info.MaxWidth != null)
				args.AppendFormat(" --max-width {0}", info.MaxWidth.Value);
			args.Append(" --platform win");

			if (!string.IsNullOrEmpty(monoPath))
			{
				// use mono to execute the command
				args.Insert(0, convertExe + " ");
				convertExe = monoPath;
			}
			// todo: logging
			// Console.WriteLine("Executing: {0} {1}", convertExe, args);

			var process = new Process
			{
				EnableRaisingEvents = true,
				StartInfo = new ProcessStartInfo
				{
					FileName = convertExe,
					Arguments = args.ToString(),
					WorkingDirectory = appPath,
					UseShellExecute = false,
					RedirectStandardOutput = true, // turn these off (with mono) to see output errors
					RedirectStandardError = true,
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