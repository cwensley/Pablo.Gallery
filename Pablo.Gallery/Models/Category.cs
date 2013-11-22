using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Pablo.Gallery.Models
{
	[Table("Category", Schema = "gallery")]
	public class Category
	{
		public int Id { get; set; }

		[MaxLength(256)]
		public string Name { get; set; }

		public virtual ICollection<Pack> Packs { get; set; }
	}
}