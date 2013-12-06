using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity;

namespace Pablo.Gallery.Models
{
	[Table("File_Content", Schema = GalleryContext.Schema)]
	public class FileContent
	{
		[Key]
		[Column("File_Id")]
		[ForeignKey("File")]
		public int FileId { get; set; }

		public virtual File File { get; set; }

		public string Text { get; set; }
	}
}