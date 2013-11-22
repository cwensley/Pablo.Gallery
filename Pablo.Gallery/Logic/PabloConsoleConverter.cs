using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Pablo.Gallery.Logic
{
	class PabloProcessConverter : ProcessConverter
	{
		public override Task Convert(string inFile, string outFile, int zoom)
		{
			var tcs = new TaskCompletionSource<object>();
			var convertExe = "PabloDraw.Console.exe";
			var appPath = HttpContext.Current.Request.MapPath("~/bin");
			convertExe = Path.Combine(appPath, convertExe);
			var args = string.Format("--convert {0} --out {1}", inFile, outFile);

			/**/
			var exeFile = "/usr/bin/mono";
			args = convertExe + " " + args;
			/**
			var exeFile = convertExe;
			/**/
			Console.WriteLine("Executing: {0} {1}", exeFile, args);

			var process = new Process
			{
				EnableRaisingEvents = true,
				StartInfo = new ProcessStartInfo
				{
					FileName = exeFile,
					Arguments = args,
					WorkingDirectory = appPath,
					UseShellExecute = false,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					CreateNoWindow = true
				}
			};

			process.Exited += (s, e) =>
			{
				Console.WriteLine("Done Conversion");
				tcs.SetResult(true);
			};
			process.OutputDataReceived += (s, e) =>
			{
				Console.WriteLine("Convert: {0}", e.Data);
			};
			process.ErrorDataReceived += (s, e) =>
			{
				Console.WriteLine("Error! " + e.Data);
			};
			process.Start();
			return tcs.Task;
		}
	}
}