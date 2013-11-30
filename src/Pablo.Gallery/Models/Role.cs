using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Pablo.Gallery.Models
{
	[Table("Role", Schema = GalleryContext.Schema)]
	public class Role
	{
		public int Id { get; set; }

		[MaxLength(30)]
		public string Name { get; set; }

		public virtual ICollection<User> Users { get; set; }
	}
}

