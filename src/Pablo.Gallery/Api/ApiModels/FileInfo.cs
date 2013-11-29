using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;

namespace Pablo.Gallery.Api.ApiModels
{
	[DataContract(Name = "file")]
	public class FileSummary
	{
		readonly Models.File file;

		public FileSummary(Models.File file)
		{
			this.file = file;
		}

		[DataMember(Name = "pack")]
		public string Pack { get { return file.Pack.Name; } }

		[DataMember(Name = "fileName")]
		public string FileName { get { return file.FileName; } }

		public string NativeFileName { get { return file.NativeFileName; } }

		public PackSummary PackSummary { get { return new PackSummary(file.Pack); } }

		public string Path
		{
			get { return HttpUtility.UrlPathEncode(Pack) + "/" + HttpUtility.UrlPathEncode(FileName).Replace("%5c", "\\").Replace("&amp;", "&"); }
		}

		public string PreferredFormat(float? zoom, float? maxWidth)
		{
			if (file.Type == "character" || file.Type == "rip")
				return "png";
			if (file.Type == "image")
			{
				if ((zoom == null || zoom >= 0.5f) && maxWidth == null)
					return "raw";
				//if (FileName.EndsWith(".gif", StringComparison.OrdinalIgnoreCase))
				//	return "gif";
				//if (FileName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) || FileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
				//	return "jpeg";
				return "png";
			}
			return null;

		}

		public string Url(UrlHelper url, float? zoom = null, int? maxWidth = null)
		{
			var format = PreferredFormat(zoom, maxWidth);
			if (format != null)
			{
				var sb = new StringBuilder();
				sb.AppendFormat("~/api/v0/pack/{0}?format={1}", Path, format);
				if (zoom != null)
					sb.AppendFormat("&zoom={0:0}", zoom * 100);
				if (maxWidth != null)
					sb.AppendFormat("&max-width={0}", maxWidth);
				return url.Content(sb.ToString());
			}
			else
			{
				var img = GetGenericImageUrl(System.IO.Path.GetExtension(NativeFileName));
				return url.Content(string.Format("~/Content/img/file-type/{0}.png", img));
			}
		}

		string GetGenericImageUrl(string extension)
		{
			switch (extension.ToLowerInvariant())
			{
				case ".exe":
				case ".com":
				case ".dll":
					return "general";
				case ".arj":
				case ".zip":
				case ".rar":
				case ".7z":
					return "compressed";
				case ".css":
					return "css";
				case ".s3m":
				case ".mod":
				case ".xm":
				case ".it":
				case ".669":
				case ".mp3":
					return "music";
				case ".html":
				case ".htm":
					return "html";
				case ".pcx":
					return "image";
				case ".fli":
				case ".mpg":
					return "movie";
				default:
					return "blank";
			}
		}

		public string RawUrl(UrlHelper url)
		{
			return url.Content(string.Format("~/api/v0/pack/{0}/{1}?format=raw", file.Pack.Name, file.FileName));
		}

		public bool UseNearestNeighbour
		{
			get { return file.UseNearestNeighbour; }
		}
	}

	[DataContract(Name = "file")]
	public class FileDetail : FileSummary
	{
		public FileDetail(Models.File file)
			: base(file)
		{
			Year = file.Pack.Date.Value.Year;
		}

		[DataMember(Name = "year")]
		public int Year { get; set; }
	}
}