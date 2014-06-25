using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Web.Security;

namespace Pablo.Gallery.Models
{
	[Table("User", Schema = GalleryContext.Schema)]
	public class User
	{
		public int Id { get; set; }

		public DateTime? CreateDate { get; set; }

		public DateTime? LastLoginDate { get; set; }

		[Index("ux_User_UserName", IsUnique = true)]
		public string UserName { get; set; }

		public string Email { get; set; }

		public string Alias { get; set; }

		//public virtual Artist Artist { get; set; }

		[NotMapped]
		public string DisplayName
		{
			get { return string.IsNullOrEmpty(Alias) ? UserName : Alias; }
		}

		[MaxLength(128)]
		public string ConfirmationToken { get; set; }

		public bool IsConfirmed { get; set; }

		public DateTime? LastPasswordFailureDate { get; set; }

		[MaxLength(128)]
		public string Password { get; set; }

		public int PasswordFailuresSinceLastSuccess { get; set; }

		public DateTime? PasswordChangedDate { get; set; }

		public string PasswordSalt { get; set; }

		[MaxLength(128)]
		public string PasswordVerificationToken { get; set; }

		public DateTime? PasswordVerificationExpiryDate { get; set; }

		public string PasswordQuestion { get; set; }

		public string PasswordAnswer { get; set; }

		public virtual ICollection<UserOAuthMembership> OAuthMemberships { get; set; }

		public virtual ICollection<Role> Roles { get; set; }

		public MembershipUser ToMembershipUser(string providerName, bool isApproved)
		{
			return new MembershipUser(providerName, UserName, Id, Email, null, null, isApproved, false, CreateDate ?? DateTime.MinValue, 
				LastLoginDate ?? DateTime.MinValue, DateTime.MinValue, PasswordChangedDate ?? DateTime.MinValue, DateTime.MinValue);
		}
	}
}

