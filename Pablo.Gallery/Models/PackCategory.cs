using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Pablo.Gallery.Models
{
	[Table("Pack_Category", Schema = "gallery")]
	public class PackCategory
	{
		[Key, Column("Pack_Id", Order = 1)]
		public int PackId { get; set; }

		[Key, Column("Category_Id", Order = 2)]
		public int CategoryId { get; set; }

		[ForeignKey("PackId")]
		public virtual Pack Pack { get; set; }

		[ForeignKey("CategoryId")]
		public virtual Category Category { get; set; }
	}
}