using System.Threading.Tasks;
using PabloDraw;
using System;
using System.Collections.Generic;

namespace Pablo.Gallery.Logic.Converters
{
	public class PabloInlineConverter : FileCacheConverter
	{
		public override bool CanConvert(ConvertInfo info)
		{
			return info.InputType == Models.FileType.Character.Name || info.InputType == Models.FileType.Image.Name || info.InputType == Models.FileType.Rip.Name;
		}

		public override Task ConvertFile(ConvertInfo info, string inFile, string outFile)
		{
			var tcs = new TaskCompletionSource<object>();

			var options = new List<ConversionOption>();
			if (info.LegacyAspect != null)
				options.Add(new ConversionOption("text-aspect", info.LegacyAspect == true ? "dos" : "none"));
			if (info.Use9x != null)
				options.Add(new ConversionOption("text-use9x", info.Use9x == true ? "true" : "false"));

			var parameters = new ConvertParameters
			{
				InputFileName = inFile,
				InputFormat = info.InputFormat,
				OutputFileName = outFile,
				MaxWidth = info.MaxWidth,
				Zoom = info.Zoom ?? 1f,
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