using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Pablo.Gallery.Models
{
	[Table("Pack", Schema = "gallery")]
	public class Pack
	{
		public int Id { get; set; }

		[MaxLength(256)]
		public string Name { get; set; }

		public string FileName { get; set; }

		public DateTime? Date { get; set; }

		public virtual ICollection<Category> Categories { get; set; }

		public virtual ICollection<File> Files { get; set; }
	}
}