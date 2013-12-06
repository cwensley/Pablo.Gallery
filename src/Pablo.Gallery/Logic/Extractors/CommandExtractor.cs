using System;
using System.IO;
using System.Collections.Generic;
using Ionic.Zip;
using System.Diagnostics;
using System.Configuration;
using System.Threading.Tasks;

namespace Pablo.Gallery.Logic.Extractors
{
	public abstract class CommandExtractor : Extractor
	{
		public override bool CanExtractInfo { get { return false; } }

		public override ExtractArchiveInfo ExtractInfo(string archiveFileName)
		{
			throw new NotSupportedException();
		}

		protected abstract string ExecutablePath { get; }
		protected abstract string GetStreamArgs(string archiveFileName, string fileName);
		protected abstract string GetDirectArgs(string archiveFileName, string fileName, string destinationDir);

		public sealed override Task<Stream> ExtractFile(string archiveFileName, string fileName)
		{
			var tcs = new TaskCompletionSource<Stream>();
			var args = GetStreamArgs(archiveFileName, fileName);

			//Console.WriteLine("Extracting file to stream: {0} {1}", GetExePath(), args);
			var process = new Process
			{
				EnableRaisingEvents = true,
				StartInfo = 
				{
					FileName = ExecutablePath,
					Arguments = args,
					UseShellExecute = false,
					LoadUserProfile = false,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					CreateNoWindow = true
				}
			};
			process.ErrorDataReceived += (sender, e) => Console.WriteLine("Error extracting! {0}", e.Data);
			process.Start();
			var memstream = new MemoryStream();
			process.Exited += (sender, e) => {
				memstream.Position = 0;
				if (process.ExitCode == 0)
				{
					tcs.SetResult(memstream);
				}
				else
					tcs.SetException(new Exception(string.Format("could not extract file (exit code {0}) Details:\n{1}", process.ExitCode, new StreamReader(memstream).ReadToEnd())));
				process.Dispose();
			};
			process.StandardOutput.BaseStream.CopyTo(memstream);
			return tcs.Task;
		}

		public sealed override Task ExtractFile(string archiveFileName, string fileName, string destinationFileName)
		{
			var destinationDir = Path.GetDirectoryName(destinationFileName);
			var destinationFile = Path.GetFileName(destinationFileName);
			if (destinationFile != fileName)
				return base.ExtractFile(archiveFileName, fileName, destinationFileName);

			var tcs = new TaskCompletionSource<bool>();
			var args = GetDirectArgs(archiveFileName, fileName, destinationDir);

			//Console.WriteLine("Extracting file: {0} {1}", GetExePath(), args);
			var process = new Process
			{
				EnableRaisingEvents = true,
				StartInfo = 
				{
					FileName = ExecutablePath,
					Arguments = args,
					UseShellExecute = false,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					CreateNoWindow = true
				}
			};
			process.Start();
			process.Exited += (sender, e) => {
				if (process.ExitCode == 0)
				{
					tcs.SetResult(true);
				}
				else
					tcs.SetException(new Exception("could not extract file"));
				process.Dispose();
			};
			return tcs.Task;
		}
	}
}


