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
using Pablo.Gallery.Logic.Extractors;
using Pablo.Gallery.Models;
using Pablo.Gallery.Logic.Filters;
using Pablo.Gallery.Logic.Selectors;

namespace Pablo.Gallery.Api.V0.Controllers
{
	public class PackController : ApiController
	{
		readonly Models.GalleryContext db = new Models.GalleryContext();

		[HttpGet]
		public PackResult Index(int page = 0, int size = Global.DefaultPageSize)
		{
			var packs = from p in db.Packs
			            orderby p.Date descending
			            select p;
			var results = packs.Skip(page * size).Take(size).AsEnumerable();
			return new PackResult
			{
				Packs = from p in results select new PackSummary(p)
			};
		}

		[HttpGet, ActionName("Index")]
		public HttpResponseMessage Download([FromUri(Name = "id")]string packName, string format)
		{
			var pack = db.Packs.FirstOrDefault(r => r.Name == packName);
			if (pack == null)
				throw new HttpResponseException(HttpStatusCode.NotFound);
			if (format != "download")
				throw new HttpResponseException(HttpStatusCode.Forbidden);

			var packArchiveFileName = Path.Combine(Global.SixteenColorsArchiveLocation, pack.NativeFileName);
			var content = new StreamContent(System.IO.File.OpenRead(packArchiveFileName));
			content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
			//content.Headers.ContentLength = pack.FileSize;
			content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
			{
				FileName = Path.GetFileName(pack.NativeFileName)
			};
			var response = new HttpResponseMessage { Content = content };
			response.Headers.CacheControl = new CacheControlHeaderValue
			{
				MaxAge = new TimeSpan(0, 10, 0),
				Public = true
			};
			return response;
		}

		[HttpGet, EnableCors]
		public PackDetail Index([FromUri(Name = "id")]string packName, int page = 0, int size = Global.DefaultPageSize)
		{
			var pack = db.Packs.FirstOrDefault(p => p.Name == packName);
			if (pack == null)
				throw new HttpResponseException(HttpStatusCode.NotFound);
			return new PackDetail(pack, page, size);
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
				case "MP3":
					return "audio/mpeg";
				case "OGG":
				case "OGA":
					return "application/ogg";
				default:
					return "application/octet-stream";
			}
		}

		[HttpGet, HttpPost, EnableCors]
		public FileDetail Index([FromUri(Name = "id")]string pack, [FromUri(Name = "path")] string name)
		{
			var file = db.Files.FirstOrDefault(r => r.Pack.Name == pack && r.Name == name);
			if (file == null)
				throw new HttpResponseException(HttpStatusCode.NotFound);
			return new FileDetail(file);
		}

		async Task GetStream(Stream outStream, string outFile, string packArchiveFileName, Models.File file, int? zoom, int? maxWidth, bool? aspect, bool? use9x, bool? ice)
		{
			using (outStream)
			{
				var extractor = ExtractorFactory.GetFileExtractor(packArchiveFileName);

				var convertInfo = new ConvertInfo
				{
					Pack = file.Pack,
					ExtractFile = async destFile => await extractor.ExtractFile(packArchiveFileName, file.FileName, destFile),
					FileName = file.NativeFileName,
					InputFormat = file.Format,
					InputType = file.Type,
					OutFileName = outFile,
					Zoom = zoom,
					MaxWidth = maxWidth,
					LegacyAspect = aspect,
					Use9x = use9x,
					IceColor = ice
				};
				var converter = ConverterFactory.GetConverter(convertInfo);
				var stream = await converter.Convert(convertInfo);
				stream.CopyTo(outStream);
			}
		}

		async Task GetRawStream(Stream outStream, string packArchiveFileName, Models.File file)
		{
			using (outStream)
			{
				var extractor = ExtractorFactory.GetFileExtractor(packArchiveFileName);
				var stream = await extractor.ExtractFile(packArchiveFileName, file.FileName);
				stream.CopyTo(outStream);
			}
		}

		[HttpGet]
		public HttpResponseMessage Index([FromUri(Name = "id")]string packName, [FromUri(Name = "path")] string name, string format, int? zoom = null, [FromUri(Name = "max-width")] int? maxWidth = null, bool? aspect = null, bool? use9x = null, bool? ice = null)
		{
			var file = db.Files.FirstOrDefault(r => r.Pack.Name == packName && r.Name == name);
			if (file == null)
				throw new HttpResponseException(HttpStatusCode.NotFound);

			var download = string.Equals(format, "download", StringComparison.OrdinalIgnoreCase);

			var packArchiveFileName = Path.Combine(Global.SixteenColorsArchiveLocation, file.Pack.NativeFileName);
			var outFile = file.NativeFileName;

			// TODO: Move most of this to the converters and file type, including per-converter options

			HttpContent content;
			if (download || (file.Type == FileType.Audio.Name && Path.GetExtension(file.NativeFileName).TrimStart('.').Equals(format, StringComparison.OrdinalIgnoreCase)))
			{
				content = new PushStreamContent((s, hc, t) => GetRawStream(s, packArchiveFileName, file));
			}
			else
			{
				if (zoom != null)
					outFile += ".z" + zoom.Value;
				if (maxWidth != null)
					outFile += ".x" + maxWidth.Value;
				if (aspect != null)
					outFile += aspect == true ? ".da" : ".na";
				if (use9x != null)
					outFile += use9x == true ? ".9x" : ".8x";
				if (ice != null)
					outFile += ice == true ? ".ice" : ".blink";
				outFile += "." + format;
				content = new PushStreamContent((s, hc, t) => GetStream(s, outFile, packArchiveFileName, file, zoom, maxWidth, aspect, use9x, ice));
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
