using Pablo.Gallery.Api.ApiModels;
using Pablo.Gallery.Logic;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using Pablo.Gallery.Logic.Converters;

namespace Pablo.Gallery.Api.V0.Controllers
{
	public class PackController : ApiController
	{
		readonly Models.GalleryContext db = new Models.GalleryContext();

		[HttpGet]
		public PackResult Index(int start = 0, int limit = 100)
		{
			var packs = from p in db.Packs
			            select p;
			return new PackResult
			{
				Packs = from p in packs.Skip(start).Take(limit).AsEnumerable()
				        select new PackSummary(p)
			};
		}

		[HttpGet]
		public PackDetail Index([FromUri(Name = "id")]string packName, int start = 0, int limit = 100)
		{
			var packs = from p in db.Packs
			            where p.Name == packName
			            select p;
			var pack = packs.FirstOrDefault();
			if (pack == null)
				throw new HttpResponseException(HttpStatusCode.NotFound);
			return new PackDetail(pack, start, limit);
		}

		static string GetMediaType(string format)
		{
			switch (format.ToUpperInvariant())
			{
				case "PNG":
					return "image/png";
				case "GIF":
					return "image/gif";
				case "JPG":
				case "JPEG":
					return "image/jpeg";
				case "TIFF":
					return "image/tiff";
				default:
					return "application/octet-stream";
			}
		}

		[HttpGet]
		public FileDetail Index([FromUri(Name = "id")]string pack, [FromUri(Name = "id2")] string fileName)
		{
			fileName = Scanner.NormalizedPath(fileName);
			var files = from f in db.Files
			            where f.Pack.Name == pack && f.FileName == fileName
			            select f;
			var file = files.FirstOrDefault();
			if (file == null)
				throw new HttpResponseException(HttpStatusCode.NotFound);
			return new FileDetail(file);
		}

		[HttpGet]
		public async Task<HttpResponseMessage> Index([FromUri(Name = "id")]string packName, [FromUri(Name = "id2")] string fileName, string format, int? zoom = null, [FromUri(Name = "max-width")] int? maxWidth = null)
		{
			fileName = Scanner.NormalizedPath(fileName);
			var file = db.Files.FirstOrDefault(r => r.Pack.Name == packName && r.FileName == fileName);
			if (file == null)
				throw new HttpResponseException(HttpStatusCode.NotFound);

			var raw = string.Equals(format, "raw", StringComparison.OrdinalIgnoreCase);

			var packArchiveFileName = Path.Combine(Global.SixteenColorsArchiveLocation, file.Pack.NativeFileName);
			var extractor = Logic.Extractors.ExtractorFactory.GetFileExtractor(packArchiveFileName);
			var outFile = file.NativeFileName;

			StreamContent content;
			if (raw)
			{
				content = new StreamContent(await extractor.ExtractFile(packArchiveFileName, file.FileName));
			}
			else
			{
				if (zoom != null)
					outFile += ".z" + zoom.Value;
				if (maxWidth != null)
					outFile += ".x" + maxWidth.Value;
				outFile += "." + format;

				var converter = ConverterFactory.GetConverter(file.FileName);
				var convertInfo = new ConvertInfo
				{
					Pack = file.Pack,
					ExtractFile = async destFile => await extractor.ExtractFile(packArchiveFileName, file.FileName, destFile),
					FileName = file.NativeFileName,
					OutFileName = outFile,
					Zoom = zoom,
					MaxWidth = maxWidth
				};
				var stream = await converter.Convert(convertInfo);
				content = new StreamContent(stream);
			}

			var mediaType = GetMediaType(format);
			content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);
			var disposition = mediaType == "application/octet-stream" ? "attachment" : "inline";
			content.Headers.ContentDisposition = new ContentDispositionHeaderValue(disposition)
			{
				FileName = Path.GetFileName(outFile)
			};

			var response = new HttpResponseMessage { Content = content };
			response.Headers.CacheControl = new CacheControlHeaderValue
			{
				MaxAge = new TimeSpan(0, 10, 0),
				Public = true
			};
			return response;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
				db.Dispose();
			base.Dispose(disposing);
		}
	}
}
