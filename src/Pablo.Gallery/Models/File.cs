using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity;
using System.Text;
using System.Web;

namespace Pablo.Gallery.Models
{
	[Table("File", Schema = GalleryContext.Schema)]
	public class File
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public string FileName { get; set; }

		[MaxLength(20)]
		public string Format { get; set; }

		[MaxLength(20)]
		public string Type { get; set; }

		public int? Order { get; set; }

		public int? Width { get; set; }

		public int? Height { get; set; }

		[NotMapped]
		public string NativeFileName
		{
			get { return Logic.Scanner.NativePath(FileName); }
		}

		public bool UseNearestNeighbour
		{
			get
			{
				return (Type == FileType.Character.Name || Type == FileType.Rip.Name);
			}
		}

		[InverseProperty("Files")]
		[Required]
		public virtual Pack Pack { get; set; }

		public virtual FileContent Content { get; set; }


		public string Path
		{
			get { return HttpUtility.UrlPathEncode(Pack.Name) + "/" + HttpUtility.UrlPathEncode(Name).Replace("%5c", "\\").Replace("&amp;", "&"); }
		}

		public string OutputType
		{
			get {
				if (Type != FileType.Audio.Name)
					return FileType.Image.Name;
				return Type;
			}
		}

		public string PreferredFormat(float? zoom, float? maxWidth, bool preview)
		{
			if (Type == FileType.Audio.Name)
			{
				if (preview)
					return null;
				return Format == "mp3" ? "download" : "mp3";
			}
			if (Type == FileType.Character.Name || Type == FileType.Rip.Name)
				return "png";
			if (Type == FileType.Image.Name)
			{
				if (Format == "icon" || Format == "pcx")
					return "png";
				if ((zoom == null || zoom >= 0.5f) && maxWidth == null)
					return "download";
				//if (FileName.EndsWith(".gif", StringComparison.OrdinalIgnoreCase))
				//	return "gif";
				//if (FileName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) || FileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
				//	return "jpeg";
				return "png";
			}
			return null;
		}

		public string PreviewUrl(float? zoom = null, int? maxWidth = null, string format = null)
		{
			format = format ?? PreferredFormat(zoom, maxWidth, true);
			if (format != null)
			{
				var sb = new StringBuilder();
				sb.AppendFormat("~/api/v0/pack/{0}?format={1}", Path, format);
				if (zoom != null)
					sb.AppendFormat("&zoom={0:0}", zoom * 100);
				if (maxWidth != null)
					sb.AppendFormat("&max-width={0}", maxWidth);
				return sb.ToString();
			}
			else
			{
				var type = Type ?? "blank";
				return string.Format("~/Content/img/file-type/{0}.png", type.ToLowerInvariant());
			}
		}

		public string Url(string format = null)
		{
			format = format ?? PreferredFormat(null, null, false);
			if (format != null)
			{
				var sb = new StringBuilder();
				sb.AppendFormat("~/api/v0/pack/{0}?format={1}", Path, format);
				return sb.ToString();
			}
			else
			{
				var type = Type ?? "blank";
				return string.Format("~/Content/img/file-type/{0}.png", type.ToLowerInvariant());
			}
		}

		public string DisplayUrl(string format = null)
		{
			format = format ?? PreferredFormat(null, null, false);
			var fileFormat = FileFormat.Find(format);
			if ((fileFormat != null && fileFormat.Type.Name == FileType.Image.Name) || Type == FileType.Image.Name)
			{
				return Url(format);
			}
			else
			{
				return string.Format("~/pack/{0}/{1}?display=true", Pack.Name, Name);
			}
		}

		public string DisplayType()
		{
			var format = PreferredFormat(null, null, false);
			if (format == null || format == "download")
				return Type;
			var fileFormat = FileFormat.Find(format);
			return fileFormat != null ? fileFormat.Type.Name : null;
		}

		public string DownloadUrl()
		{
			return string.Format("~/api/v0/pack/{0}/{1}?format=download", Pack.Name, Name);
		}
	}
}