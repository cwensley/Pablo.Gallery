using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Pablo.Gallery.Api.ApiModels
{
	[DataContract(Name = "pack")]
	public class PackSummary
	{
		public static PackSummary FromModel(Models.Pack p)
		{
			return new PackSummary
			{
				Name = p.Name,
				Date = p.Date,
				FileName = p.FileName
			};
		}

		[DataMember(Name = "name")]
		public string Name { get; set; }

		[DataMember(Name = "date")]
		public DateTime? Date { get; set; }

		[DataMember(Name = "groups")]
		public string[] Groups { get; set; }

		[DataMember(Name = "fileName")]
		public string FileName { get; set; }
	}

	[DataContract(Name = "pack")]
	public class PackDetail : PackSummary
	{
		public new static PackDetail FromModel(Models.Pack p)
		{
			return new PackDetail
			{
				Name = p.Name,
				Date = p.Date,
				FileName = p.FileName,
			};
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