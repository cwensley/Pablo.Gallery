using System.Threading.Tasks;
using PabloDraw;
using System;

namespace Pablo.Gallery.Logic.Converters
{
	public class PabloInlineConverter : FileCacheConverter
	{
		public override Task ConvertFile(ConvertInfo info, string inFile, string outFile)
		{
			var tcs = new TaskCompletionSource<object>();

			var parameters = new ConvertParameters
			{
				InputFileName = inFile,
				OutputFileName = outFile,
				MaxWidth = info.MaxWidth,
				Zoom = info.Zoom ?? 1f
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