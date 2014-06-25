using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Text;
using System.Configuration;
using System.Linq;
using System.Collections.Generic;

namespace Pablo.Gallery.Logic.Converters
{
	class AudioConverter : FileCacheConverter
	{
		readonly string ffmpegPath;

		public AudioConverter(string ffmpegPath = null)
		{
			this.ffmpegPath = ffmpegPath ?? ConfigurationManager.AppSettings["FFmpegPath"];
		}

		static readonly string[] inputFormats = { ".mp3", ".ogg", ".oga", ".voc" };
		static readonly string[] outputFormats = { ".mp3", ".ogg", ".oga" };

		public override bool CanConvert(ConvertInfo info)
		{
			if (string.IsNullOrEmpty(ffmpegPath))
				return false;
			var inExt = Path.GetExtension(info.FileName).ToLowerInvariant();
			var outExt = Path.GetExtension(info.OutFileName).ToLowerInvariant();
			return info.InputType == Models.FileType.Audio.Name
				&& inputFormats.Contains(inExt)
				&& outputFormats.Contains(outExt)
				&& !string.Equals(inExt, outExt, StringComparison.OrdinalIgnoreCase);
		}

		public override Task ConvertFile(ConvertInfo info, string inFile, string outFile)
		{
			var tcs = new TaskCompletionSource<object>();
			var args = new StringBuilder();
			args.AppendFormat("-i \"{0}\"", inFile);
			if (outFile.EndsWith(".ogg", StringComparison.OrdinalIgnoreCase))
				args.Append(" -acodec libvorbis");
			args.AppendFormat(" \"{0}\"", outFile);


			Console.WriteLine("Executing: {0} {1}", ffmpegPath, args);
			var process = new Process
			{
				EnableRaisingEvents = true,
				StartInfo = new ProcessStartInfo
				{
					FileName = ffmpegPath,
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