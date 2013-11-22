using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Pablo.Gallery.Logic
{
	class ConvertInfo
	{
		public string Pack { get; set; }
		public Func<Stream> InputStream { get; set; }
		public string FileName { get; set; }
		public Stream OutputStream { get; set; }
		public string OutFileName { get; set; }

		public int Zoom { get; set; }
	}

	abstract class Converter
	{
		public abstract Task Convert(ConvertInfo info);
	}
	static class TaskExtensions
	{
		public static Task<T> FromResult<T>(T result)
		{
			var tcs = new TaskCompletionSource<T>();
			tcs.SetResult(result);
			return tcs.Task;
		}
	}

	abstract class ProcessConverter : Converter
	{
		public abstract Task Convert(string inFile, string outFile, int zoom);

		public override Task Convert(ConvertInfo info)
		{
			var tcs = new TaskCompletionSource<object>();
			//var cacheDir = "/Users/curtis/Downloads/pablo/cache";
			var cacheDir = @"z:\Downloads\pablo\cache";
			var packDir = Path.Combine(cacheDir, info.Pack);
			Directory.CreateDirectory(packDir);

			var inFileName = Path.Combine(packDir, info.FileName);
			if (!File.Exists(inFileName))
			{
				using (var inputStream = info.InputStream())
				{
					if (inputStream == null)
						throw new Exception("Invalid input file");
					using (var file = File.Create(inFileName))
					{
						inputStream.CopyTo(file);
					}
				};
			}
			var outFileName = Path.Combine(packDir, info.OutFileName);
			if (!File.Exists(outFileName))
			{
				Convert(inFileName, outFileName, info.Zoom).ContinueWith(t =>
				{
					//Console.WriteLine("Converted!!");
					if (t.IsCompleted && File.Exists(outFileName))
					{
						using (var file = File.OpenRead(outFileName))
						{
							file.CopyTo(info.OutputStream);
						}
					}
					tcs.SetResult(true);
				});
			}
			else
			{
				using (var file = File.OpenRead(outFileName))
				{
					file.CopyTo(info.OutputStream);
				}
				tcs.SetResult(true);
			}
			return tcs.Task;
		}
	}

	/**
	var outputStream = new MemoryStream();
	using (var memStream = new MemoryStream())
	{
		var inStream = entry.OpenEntryStream();
		inStream.CopyTo(memStream);
		memStream.Position = 0;
		var pablo = new PabloEngine();
		pablo.Convert(memStream, file, outputStream, outFile);
		outputStream.Position = 0;
	}
	var streamOutput = new StreamContent(outputStream);
	/**
	var memStream = new MemoryStream();
	using (var inStream = entry.OpenEntryStream())
	{
		inStream.CopyTo(memStream);
	}
	memStream.Position = 0;
	var streamOutput = new PushStreamContent((outputStream, content, context) =>
	{
		pablo.Convert(memStream, file, outputStream, outFile, zoom / 100f);

		memStream.Dispose();
		outputStream.Close();
	});
	/**/

}