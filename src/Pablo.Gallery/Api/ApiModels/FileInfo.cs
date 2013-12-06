using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using Pablo.Gallery.Models;

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

		[DataMember(Name = "url")]
		public string Url { get { return "pack/" + file.Path; } }

		[DataMember(Name = "downloadUrl")]
		public string DownloadUrl { get { return file.DownloadUrl().TrimStart('~'); } }

		[DataMember(Name = "previewUrl")]
		public string PreviewUrl { get { return file.PreviewUrl(maxWidth: 320).TrimStart('~'); } }

		[DataMember(Name = "pack")]
		public string Pack { get { return file.Pack.Name; } }

		[DataMember(Name = "path")]
		public string Path { get { return Logic.Scanner.NormalizedPath(System.IO.Path.GetDirectoryName(file.NativeFileName)); } }

		[DataMember(Name = "fileName")]
		public string FileName { get { return System.IO.Path.GetFileName(file.NativeFileName); } }

		[DataMember(Name = "name")]
		public string Name { get { return file.Name; } }

		[DataMember(Name = "format")]
		public string Format { get { return file.Format; } }

		[DataMember(Name = "type")]
		public string Type { get { return file.Type; } }

		[DataMember(Name = "displayUrl")]
		public string DisplayUrl { get { return file.DisplayUrl().TrimStart('~'); } }

		[DataMember(Name = "displayType")]
		public string DisplayType { get { return file.DisplayType(); } }
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