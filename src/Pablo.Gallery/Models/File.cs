using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pablo.Gallery.Models
{
	[Table("File", Schema = GalleryContext.Schema)]
	public class File
	{
		public int Id { get; set; }

		public string FileName { get; set; }

		[MaxLength(20)]
		public string Format { get; set; }

		[MaxLength(20)]
		public string Type { get; set; }

		public int? Order { get; set; }

		public int? Width { get; set; }

		public int? Height { get; set; }

		[NotMapped]
		public string NativeFileName
		{
			get { return Logic.Scanner.NativePath(FileName); }
		}

		public bool UseNearestNeighbour
		{
			get
			{
				return (Type == "character" || Type == "rip");
			}
		}

		[InverseProperty("Files")]
		[Required]
		public virtual Pack Pack { get; set; }
	}
}