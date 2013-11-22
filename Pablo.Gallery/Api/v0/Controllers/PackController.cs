using Pablo.Gallery.Api.ApiModels;
using Pablo.Gallery.Logic;
using PabloDraw;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Pablo.Gallery.Api.v0.Controllers
{
	public class PackController : ApiController
	{
		Models.GalleryContext db = new Models.GalleryContext();

		[HttpGet]
		public PackResult Index()
		{
			var packs = from p in db.Packs
						select p;
			return new PackResult
			{
				Packs = from p in packs.AsEnumerable() select PackSummary.FromModel(p)
			};
		}

		[HttpGet]
		public PackDetail Index([FromUri(Name = "id")]string packName)
		{
			var packs = from p in db.Packs
						where p.Name == packName
						select p;
			return PackDetail.FromModel(packs.FirstOrDefault());
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

		//static PabloEngine pablo = new PabloEngine("win");

		[HttpGet]
		public HttpResponseMessage Index([FromUri(Name = "id")]string pack, [FromUri(Name = "id2")] string file, string format, int zoom = 100)
		{
			var extractor = new Extractor();

			var outFile = file + "." + format;
			var streamOutput = new PushStreamContent((outputStream, content, context) =>
			{
				var converter = new PabloProcessConverter();
				converter.Convert(new ConvertInfo
				{
					Pack = pack,
					InputStream = () => extractor.Extract(pack, file),
					FileName = file,
					OutputStream = outputStream,
					OutFileName = outFile,
					Zoom = zoom
				}).Wait();
				outputStream.Close();
			});
			/**/

			var mediaType = GetMediaType(format);
			streamOutput.Headers.ContentType = new MediaTypeHeaderValue(mediaType);
			var disposition = mediaType == "application/octet-stream" ? "attachment" : "inline";
			streamOutput.Headers.ContentDisposition = new ContentDispositionHeaderValue(disposition)
			{
				FileName = outFile
			};

			return new HttpResponseMessage
			{
				Content = streamOutput
			};
			return new HttpResponseMessage(HttpStatusCode.NotFound);
		}
	}
}
