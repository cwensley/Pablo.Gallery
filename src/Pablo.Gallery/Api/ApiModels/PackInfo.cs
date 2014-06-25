using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Web.Mvc;

namespace Pablo.Gallery.Api.ApiModels
{
	class DateOnlyConverter : IsoDateTimeConverter
	{
		public DateOnlyConverter()
		{
			DateTimeFormat = "yyyy-MM-dd";
		}
	}

	[DataContract(Name = "pack")]
	public class PackSummary
	{
		Models.Pack pack;

		public PackSummary(Models.Pack pack)
		{
			this.pack = pack;
		}

		[DataMember(Name = "url")]
		public string Url { get { return "pack/" + pack.Name; } }

		[DataMember(Name = "previewUrl")]
		public string PreviewUrl { get { return pack.PreviewUrl(maxWidth: 320).TrimStart('~'); } set { } }

		[DataMember(Name = "name")]
		public string Name { get { return pack.Name; } }

		[DataMember(Name = "date")]
		[JsonConverter(typeof(DateOnlyConverter))]
		public DateTime? Date { get { return pack.Date; } }

		[DataMember(Name = "groups")]
		public string[] Groups { get; set; }

		[DataMember(Name = "fileName")]
		public string FileName { get { return pack.FileName; } }

		[DataMember(Name = "thumbnail")]
		public FileSummary Thumbnail
		{
			get { return pack.Thumbnail != null ? new FileSummary(pack.Thumbnail) : null; }
			set { }
		}
	}

	[DataContract(Name = "pack")]
	public class PackDetail : PackSummary
	{
		public PackDetail(Models.Pack p, int page = 0, int size = Global.DefaultPageSize)
			: base(p)
		{
			Files = (from f in p.Files
			        orderby f.Order
				select new FileSummary(f)).Skip(page * size).Take(size);
		}

		[DataMember(Name = "files")]
		public IEnumerable<FileSummary> Files { get; set; }
	}

	[DataContract(Name = "result")]
	public class PackResult
	{
		[DataMember(Name = "packs")]
		public IEnumerable<PackSummary> Packs { get; set; }
	}
}