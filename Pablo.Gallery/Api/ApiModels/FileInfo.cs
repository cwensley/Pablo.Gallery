using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Pablo.Gallery.Api.ApiModels
{
	[DataContract(Name = "file")]
	public class FileSummary
	{
		[DataMember(Name = "fileName")]
		public string FileName { get; set; }
	}

	public class FileDetail : FileSummary
	{

	}
}