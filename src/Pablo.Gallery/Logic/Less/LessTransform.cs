using dotless.Core.Input;
using System.IO;
using System.Web.Hosting;
using System.Web.Optimization;
using dotless.Core.configuration;

namespace Pablo.Gallery.Logic.Less
{
	sealed class VirtualFileReader : IFileReader
	{
		public byte[] GetBinaryFileContents(string fileName)
		{
			fileName = GetFullPath(fileName);
			return File.ReadAllBytes(fileName);
		}

		public string GetFileContents(string fileName)
		{
			fileName = GetFullPath(fileName);
			return File.ReadAllText(fileName);
		}

		public bool DoesFileExist(string fileName)
		{
			fileName = GetFullPath(fileName);
			return File.Exists(fileName);
		}

		static string GetFullPath(string path)
		{
			return HostingEnvironment.MapPath("~/Content/" + path);
		}

		public bool UseCacheDependencies
		{
			get { return true; }
		}
	}

	public class LessTransform : IBundleTransform
	{
		public void Process(BundleContext context, BundleResponse response)
		{
			var config = new DotlessConfiguration();
			config.MinifyOutput = false;
			config.ImportAllFilesAsLess = true;
			config.CacheEnabled = false;
			config.LessSource = typeof(VirtualFileReader);
			#if DEBUG
			//config.Logger = typeof(DiagnosticsLogger);
			#endif

			response.Content = dotless.Core.Less.Parse(response.Content, config);
			response.ContentType = "text/css";
		}
	}
}

