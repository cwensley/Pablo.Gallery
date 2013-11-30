using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Pablo.Gallery.Models
{
	[Table("User_OAuthMembership", Schema = GalleryContext.Schema)]
	public class UserOAuthMembership
	{
		public virtual User User { get; set; }

		[MaxLength(30), Key, Column(Order = 0)]
		public string Provider { get; set; }

		[MaxLength(100), Key, Column(Order = 1)]
		public string ProviderUserId { get; set; }
	}
}

