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
			Name = pack.Name;
			Date = pack.Date;
			FileName = pack.FileName;
		}

		[DataMember(Name = "name")]
		public string Name { get; set; }

		[DataMember(Name = "date")]
		[JsonConverter(typeof(DateOnlyConverter))]
		public DateTime? Date { get; set; }

		[DataMember(Name = "groups")]
		public string[] Groups { get; set; }

		[DataMember(Name = "fileName")]
		public string FileName { get; set; }

		public FileSummary Thumbnail
		{
			get { return pack.Thumbnail != null ? new FileSummary(pack.Thumbnail) : null; }
		}

		public string Url(float? zoom = null, int? maxWidth = null)
		{
			if (Thumbnail != null)
			{
				return Thumbnail.Url(zoom, maxWidth);
			}
			else
			{
				return "~/Content/img/blank.png";
			}
		}
	}

	[DataContract(Name = "pack")]
	public class PackDetail : PackSummary
	{
		public PackDetail(Models.Pack p, int start = 0, int count = int.MaxValue)
			: base(p)
		{
			Files = (from f in p.Files
			        orderby f.Order
				select new FileSummary(f)).Skip(start).Take(count);
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