using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pablo.Gallery.Models
{
	[Table("Category", Schema = GalleryContext.Schema)]
	public class Category
	{
		public int Id { get; set; }

		[MaxLength(256)]
		public string Name { get; set; }

		public virtual ICollection<Pack> Packs { get; set; }
	}
}