using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Pablo.Gallery.Models
{
	[Table("File", Schema = "gallery")]
	public class File
	{
		public int Id { get; set; }

		[MaxLength(256)]
		public string Name { get; set; }

		public string FileName { get; set; }

		//public DateTime? Date { get; set; }

		public virtual Pack Pack { get; set; }
	}
}