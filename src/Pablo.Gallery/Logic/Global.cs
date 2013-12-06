using System.Configuration;
using PabloDraw;

namespace Pablo.Gallery
{
	public static class Global
	{
		public static void Initialize()
		{
			// using windows (system.drawing) works better in a server environment, albeit a wee slower on a mac
			PabloEngine = new PabloEngine("win"); 
		}

		public const int DefaultPageSize = 100;

		public static PabloEngine PabloEngine { get; private set; }


		static string sixteenColorsArchiveLocation;

		public static string SixteenColorsArchiveLocation
		{
			get { return sixteenColorsArchiveLocation ?? (sixteenColorsArchiveLocation = ConfigurationManager.AppSettings["16c:ArchiveLocation"]); }
		}

		static string sixteenColorsCacheLocation;

		public static string SixteenColorsCacheLocation
		{
			get { return sixteenColorsCacheLocation ?? (sixteenColorsCacheLocation = ConfigurationManager.AppSettings["16c:CacheLocation"]); }
		}
	}
}

