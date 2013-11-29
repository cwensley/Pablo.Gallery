﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Pablo.Gallery.Models
{
	[Table("Pack", Schema = GalleryContext.Schema)]
	public class Pack
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[MaxLength(256)]
		public string Name { get; set; }

		public string FileName { get; set; }

		public virtual File Thumbnail { get; set; }

		[NotMapped]
		public string NativeFileName
		{
			get { return Logic.Scanner.NativePath(FileName); }
			set { FileName = Logic.Scanner.NormalizedPath(value); }
		}

		public File GetFileByName(string fileName, bool createIfNotExists = true)
		{
			if (Files == null)
				Files = new List<File>();
			var files = from f in Files
						where f.FileName == fileName
						select f;
			var file = files.FirstOrDefault();
			if (file == null && createIfNotExists)
			{
				file = new Models.File { Pack = this, FileName = fileName };
				this.Files.Add(file);
			}
			return file;
		}

		public DateTime? Date { get; set; }

		public virtual ICollection<Category> Categories { get; set; }

		[InverseProperty("Pack")]
		public virtual ICollection<File> Files { get; set; }
	}
}