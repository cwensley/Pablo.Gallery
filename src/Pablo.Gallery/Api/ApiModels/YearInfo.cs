using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Pablo.Gallery.Api.ApiModels
{
	[DataContract(Name = "year")]
	public class YearBase
	{
		[DataMember(Name = "year")]
		public int Year { get; set; }
	}

	[DataContract(Name = "year")]
	public class YearSummary : YearBase
	{
		[DataMember(Name = "packs")]
		public int Packs { get; set; }

		[DataMember(Name = "url")]
		public string Url { get; set; }

		public YearSummary(int year, int packs)
		{
			Year = year;
			Url = "year/" + year;
			Packs = packs;
		}
	}

	[DataContract(Name = "year")]
	public class YearDetail : YearBase
	{
		[DataMember(Name = "packs")]
		public IEnumerable<PackSummary> Packs { get; set; }
	}

	[DataContract(Name = "result")]
	public class YearResult
	{
		[DataMember(Name = "years")]
		public IEnumerable<YearSummary> Years { get; set; }
	}

}