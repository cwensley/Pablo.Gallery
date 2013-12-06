using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Text;
using System.Configuration;
using System.Linq;

namespace Pablo.Gallery.Logic.Converters
{
	class ModConverter : FileCacheConverter
	{
		readonly string xmpPath;
		readonly string ffmpegPath;

		public ModConverter(string xmpPath = null, string ffmpegPath = null)
		{
			this.xmpPath = xmpPath ?? ConfigurationManager.AppSettings["XmpPath"];
			this.ffmpegPath = ffmpegPath ?? ConfigurationManager.AppSettings["FFmpegPath"];
		}

		static readonly string[] inputFormats = { ".s3m", ".669", ".mtm", ".it", ".mod", ".stm", ".rtm", ".far", ".ult", ".xm"};
		static readonly string[] outputFormats = { ".mp3", ".oga", ".ogg"};

		public override bool CanConvert(ConvertInfo info)
		{
			if (string.IsNullOrEmpty(ffmpegPath) || string.IsNullOrEmpty(xmpPath))
				return false;
			return info.InputType == Models.FileType.Audio.Name
				&& inputFormats.Contains(Path.GetExtension(info.FileName).ToLowerInvariant())
				&& outputFormats.Contains(Path.GetExtension(info.OutFileName).ToLowerInvariant());
		}

		public override Task ConvertFile(ConvertInfo info, string inFile, string outFile)
		{
			var tcs = new TaskCompletionSource<object>();
			var args = new StringBuilder();
			args.AppendFormat("-c '{0} -d raw \"{1}\" -Dendian=little -o - ", xmpPath, inFile);
			args.AppendFormat("| {0} -f s16be -ar 44100 -ac 2 -i -", ffmpegPath);
			if (outFile.EndsWith(".ogg", StringComparison.OrdinalIgnoreCase))
				args.Append(" -acodec libvorbis");
			args.AppendFormat(" \"{0}\"", outFile);
			args.Append("'");
			var convertExe = "/bin/sh";
			Console.WriteLine("Executing: {0} {1}", convertExe, args);
			var process = new Process
			{
				EnableRaisingEvents = true,
				StartInfo = new ProcessStartInfo
				{
					FileName = convertExe,
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