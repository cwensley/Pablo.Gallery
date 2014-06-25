using System.Threading.Tasks;
using PabloDraw;
using System;
using System.Collections.Generic;
using System.IO;

namespace Pablo.Gallery.Logic.Converters
{
	public class PabloInlineConverter : FileCacheConverter
	{
		public override bool CanConvert(ConvertInfo info)
		{
			return info.InputType == Models.FileType.Character.Name || info.InputType == Models.FileType.Image.Name || info.InputType == Models.FileType.Rip.Name;
		}

		public override void Prepare(ConvertInfo info)
		{
			base.Prepare(info);

			var outFile = Path.GetFileNameWithoutExtension(info.OutFileName);
			var zoom = info.GetProperty<int?>("zoom");
			if (zoom != null)
				outFile += ".z" + zoom.Value;
			var maxWidth = info.GetProperty<int?>("max-width");
			if (maxWidth != null)
				outFile += ".x" + maxWidth.Value;
			var aspect = info.GetProperty<bool?>("aspect");
			if (aspect != null)
				outFile += aspect == true ? ".da" : ".na";
			var use9x = info.GetProperty<bool?>("use9x");
			if (use9x != null)
				outFile += use9x == true ? ".9x" : ".8x";
			var ice = info.GetProperty<bool?>("ice");
			if (ice != null)
				outFile += ice == true ? ".ice" : ".blink";
			info.OutFileName = outFile + Path.GetExtension(info.OutFileName);
		}

		public override Task ConvertFile(ConvertInfo info, string inFile, string outFile)
		{
			var tcs = new TaskCompletionSource<object>();

			var options = new List<ConversionOption>();
			var aspect = info.GetProperty<bool?>("aspect");
			var use9x = info.GetProperty<bool?>("use9x");
			var ice = info.GetProperty<bool?>("ice");
			var zoom = info.GetProperty<int?>("zoom");
			var maxWidth = info.GetProperty<int?>("max-width");

			if (aspect != null)
				options.Add(new ConversionOption("text-aspect", aspect == true ? "dos" : "none"));
			if (use9x != null)
				options.Add(new ConversionOption("text-use9x", use9x == true ? "true" : "false"));

			var parameters = new ConvertParameters
			{
				InputFileName = inFile,
				InputFormat = info.InputFormat,
				OutputFileName = outFile,
				MaxWidth = maxWidth,
				Zoom = zoom ?? 1f,
				Options = options
			};

			try
			{
				Global.PabloEngine.Convert(parameters);
				tcs.SetResult(null);
			}
			catch (Exception ex)
			{
				tcs.SetException(ex);
			}

			return tcs.Task;
		}
	}
}