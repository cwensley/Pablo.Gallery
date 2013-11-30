using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebMatrix.WebData;
using System.Security.Cryptography;
using System.Configuration.Provider;
using System.Web.Security;

namespace Pablo.Gallery.Logic.Membership
{
	public class GalleryMembershipProvider : ExtendedMembershipProvider
	{
		Models.GalleryContext Database()
		{
			return new Models.GalleryContext();
		}

		static string GenerateToken()
		{
			using (var generator = new RNGCryptoServiceProvider())
			{
				var array = new byte[16];
				generator.GetBytes(array);
				return HttpServerUtility.UrlTokenEncode(array);
			}
		}

		public override bool ConfirmAccount(string accountConfirmationToken)
		{
			using (var db = Database())
			{
				var user = db.Users.FirstOrDefault(r => r.ConfirmationToken == accountConfirmationToken);
				if (user == null)
					return false;
				user.IsConfirmed = true;
				db.SaveChanges();
				return true;
			}
		}

		public override bool ConfirmAccount(string userName, string accountConfirmationToken)
		{
			using (var db = Database())
			{
				return db.Users.Any(r => r.UserName == userName && r.ConfirmationToken == accountConfirmationToken);
			}
		}

		public override string CreateAccount(string userName, string password, bool requireConfirmationToken)
		{
			return CreateUserAndAccount(userName, password, requireConfirmationToken, null);
		}

		public override string CreateUserAndAccount(string userName, string password, bool requireConfirmation, IDictionary<string, object> values)
		{
			using (var db = Database())
			{
				var token = requireConfirmation ? GenerateToken() : null;
				var date = DateTime.UtcNow;
				var user = new Models.User
				{
					CreateDate = date,
					PasswordChangedDate = date,
					UserName = userName,
					Password = password,
					ConfirmationToken = token,
					IsConfirmed = !requireConfirmation
				};
				if (values != null)
				{
					object val;
					if (values.TryGetValue("Alias", out val) && val is string)
						user.Alias = (string)val;

				}
				db.Users.Add(user);
				db.SaveChanges();
				return token;
			}
		}

		public override bool DeleteAccount(string userName)
		{
			using (var db = Database())
			{
				var user = db.Users.FirstOrDefault(r => r.UserName == userName);
				if (user == null)
					return false;
				db.Users.Remove(user);
				db.SaveChanges();
				return true;
			}
		}

		public override string GeneratePasswordResetToken(string userName, int tokenExpirationInMinutesFromNow)
		{
			using (var db = Database())
			{
				var user = db.Users.FirstOrDefault(r => r.UserName == userName);
				if (user == null)
					throw new ProviderException("User not found");
				var token = GenerateToken();
				user.PasswordVerificationToken = token;
				user.PasswordVerificationExpiryDate = DateTime.UtcNow.AddMinutes(tokenExpirationInMinutesFromNow);
				db.SaveChanges();
				return token;
			}
		}

		public override ICollection<OAuthAccountData> GetAccountsForUser(string userName)
		{
			using (var db = Database())
			{
				var user = db.Users.FirstOrDefault(r => r.UserName == userName);

				if (user != null && user.OAuthMemberships != null)
					return user.OAuthMemberships.Select(r => new OAuthAccountData(r.Provider, r.ProviderUserId)).ToArray();
			}
			return new OAuthAccountData[0];
		}

		public override DateTime GetCreateDate(string userName)
		{
			using (var db = Database())
			{
				var user = db.Users.FirstOrDefault(r => r.UserName == userName);
				return user.CreateDate ?? DateTime.MinValue;
			}
		}

		public override DateTime GetLastPasswordFailureDate(string userName)
		{
			using (var db = Database())
			{
				var user = db.Users.FirstOrDefault(r => r.UserName == userName);
				return user.LastPasswordFailureDate ?? DateTime.MinValue;
			}
		}

		public override DateTime GetPasswordChangedDate(string userName)
		{
			using (var db = Database())
			{
				var user = db.Users.FirstOrDefault(r => r.UserName == userName);
				return user.PasswordChangedDate ?? DateTime.MinValue;
			}
		}

		public override int GetPasswordFailuresSinceLastSuccess(string userName)
		{
			using (var db = Database())
			{
				var user = db.Users.FirstOrDefault(r => r.UserName == userName);
				return user.PasswordFailuresSinceLastSuccess;
			}
		}

		public override int GetUserIdFromPasswordResetToken(string token)
		{
			using (var db = Database())
			{
				var user = db.Users.FirstOrDefault(r => r.PasswordVerificationToken == token);
				return user.Id;
			}
		}

		public override bool IsConfirmed(string userName)
		{
			using (var db = Database())
			{
				var user = db.Users.FirstOrDefault(r => r.UserName == userName);
				return user.IsConfirmed;
			}
		}

		public override bool ResetPasswordWithToken(string token, string newPassword)
		{
			using (var db = Database())
			{
				var user = db.Users.FirstOrDefault(r => r.PasswordVerificationToken == token);
				if (user == null)
					return false;
				user.Password = newPassword;
				user.PasswordChangedDate = DateTime.UtcNow;
				return true;
			}
		}

		public override string ApplicationName
		{
			get; set; 
		}

		public override bool ChangePassword(string userName, string oldPassword, string newPassword)
		{
			using (var db = Database())
			{
				var user = db.Users.FirstOrDefault(r => r.UserName == userName);
				if (user == null || oldPassword != user.Password)
					return false;
				user.Password = newPassword;
				user.PasswordChangedDate = DateTime.UtcNow;
				db.SaveChanges();
				return true;
			}
		}

		public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
		{
			throw new NotImplementedException();
		}

		public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
		{
			using (var db = Database())
			{
				var date = DateTime.UtcNow;
				var user = db.Users.FirstOrDefault(r => r.UserName == username || r.Email == email);
				if (user == null)
				{
					user = new Models.User
					{
						CreateDate = date,
						PasswordChangedDate = date,
						UserName = username,
						Password = password,
					};
					db.SaveChanges();
					status = MembershipCreateStatus.Success;
					return user.ToMembershipUser(Name, isApproved);
				}
				if (user.UserName == username)
					status = MembershipCreateStatus.DuplicateUserName;
				else if (user.Email == email)
					status = MembershipCreateStatus.DuplicateEmail;
				else
					status = MembershipCreateStatus.ProviderError;
				return null;
			}
		}

		public override bool DeleteUser(string username, bool deleteAllRelatedData)
		{
			using (var db = Database())
			{
				var user = db.Users.FirstOrDefault(r => r.UserName == username);
				if (user == null)
					return false;
				db.Users.Remove(user);
				db.SaveChanges();
				return true;
			}
		}

		public override bool EnablePasswordReset
		{
			get { return true; }
		}

		public override bool EnablePasswordRetrieval
		{
			get { return true; }
		}

		public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
		{
			using (var db = Database())
			{
				var result = new MembershipUserCollection();
				var users = db.Users.Where(r => r.Email == emailToMatch);
				totalRecords = users.Count();
				foreach (var user in users.Skip(pageIndex * pageSize).Take(pageSize))
				{
					result.Add(user.ToMembershipUser(Name, true));
				}
				return result;
			}
		}

		public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
		{
			using (var db = Database())
			{
				var result = new MembershipUserCollection();
				var users = db.Users.Where(r => r.UserName == usernameToMatch);
				totalRecords = users.Count();
				foreach (var user in users.Skip(pageIndex * pageSize).Take(pageSize))
				{
					result.Add(user.ToMembershipUser(Name, true));
				}
				return result;
			}
		}

		public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
		{
			using (var db = Database())
			{
				var result = new MembershipUserCollection();
				var users = db.Users.Skip(pageIndex * pageSize).Take(pageSize);
				totalRecords = db.Users.Count();
				foreach (var user in users)
				{
					result.Add(user.ToMembershipUser(Name, true));
				}
				return result;
			}
		}

		public override int GetNumberOfUsersOnline()
		{
			return 0;
		}

		public override string GetPassword(string username, string answer)
		{
			throw new NotImplementedException();
		}

		public override MembershipUser GetUser(string username, bool userIsOnline)
		{
			using (var db = Database())
			{
				var user = db.Users.FirstOrDefault(r => r.UserName == username);
				if (user == null)
					return null;
				return user.ToMembershipUser(Name, true);
			}
		}

		public override int GetUserIdFromOAuth(string provider, string providerUserId)
		{
			using (var db = Database())
			{
				var oauth = db.UserOAuthMemberships.FirstOrDefault(r => r.Provider == provider && r.ProviderUserId == providerUserId);
				if (oauth == null)
					return -1;
				return oauth.User.Id;
			}
		}

		public override void DeleteOAuthAccount(string provider, string providerUserId)
		{
			using (var db = Database())
			{
				var oauth = db.UserOAuthMemberships.FirstOrDefault(r => r.Provider == provider && r.ProviderUserId == providerUserId);
				if (oauth != null)
				{
					db.UserOAuthMemberships.Remove(oauth);
					db.SaveChanges();
				}
			}
		}

		public override void CreateOrUpdateOAuthAccount(string provider, string providerUserId, string userName)
		{
			using (var db = Database())
			{
				var user = db.Users.FirstOrDefault(r => r.UserName == userName);
				if (user == null)
					throw new MembershipCreateUserException(MembershipCreateStatus.InvalidUserName);
				var oauth = db.UserOAuthMemberships.FirstOrDefault(r => r.Provider == provider && r.ProviderUserId == providerUserId);
				if (oauth != null)
				{
					oauth.User = user;
				}
				else
				{
					oauth = new Models.UserOAuthMembership { User = user, Provider = provider, ProviderUserId = providerUserId };
					db.UserOAuthMemberships.Add(oauth);
				}
				db.SaveChanges();
			}
		}

		public override string GetUserNameFromId(int userId)
		{
			using (var db = Database())
			{
				var user = db.Users.FirstOrDefault(r => r.Id == userId);
				if (user != null)
					return user.UserName;
			}
			return null;
		}

		public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
		{
			throw new NotImplementedException();
		}

		public override string GetUserNameByEmail(string email)
		{
			throw new NotImplementedException();
		}

		public override int MaxInvalidPasswordAttempts
		{
			get { return 5; }
		}

		public override int MinRequiredNonAlphanumericCharacters
		{
			get { return 0; }
		}

		public override int MinRequiredPasswordLength
		{
			get { return 5; }
		}

		public override int PasswordAttemptWindow
		{
			get { return 10; }
		}

		public override MembershipPasswordFormat PasswordFormat
		{
			get { return MembershipPasswordFormat.Hashed; }
		}

		public override string PasswordStrengthRegularExpression
		{
			get { return null; }
		}

		public override bool RequiresQuestionAndAnswer
		{
			get { return false; }
		}

		public override bool RequiresUniqueEmail
		{
			get { return true; }
		}

		public override string ResetPassword(string username, string answer)
		{
			throw new NotSupportedException();
		}

		public override bool UnlockUser(string userName)
		{
			throw new NotImplementedException();
		}

		public override void UpdateUser(MembershipUser user)
		{
			throw new NotImplementedException();
		}

		public override bool ValidateUser(string username, string password)
		{
			using (var db = Database())
			{
				var user = db.Users.FirstOrDefault(r => r.UserName == username && r.Password == password && r.IsConfirmed);
				if (user == null)
					return false;
				user.LastLoginDate = DateTime.UtcNow;
				db.SaveChanges();
				return true;
			}
		}

		public override bool HasLocalAccount(int userId)
		{
			using (var db = Database())
			{
				return db.Users.Any(r => r.Id == userId);
			}
		}
	}
}