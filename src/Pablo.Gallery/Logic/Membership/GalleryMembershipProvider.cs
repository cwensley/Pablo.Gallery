using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebMatrix.WebData;
using System.Security.Cryptography;
using System.Configuration.Provider;
using System.Web.Security;
using System.IO;
using System.Text;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Collections.Specialized;

namespace Pablo.Gallery.Logic.Membership
{
	public class GalleryMembershipProvider : ExtendedMembershipProvider
	{
		MembershipPasswordFormat passwordFormat;
		MachineKeySection machineKey;
		int maxInvalidPasswordAttempts;
		int minRequiredNonAlphanumericCharacters;
		int minRequiredPasswordLength;
		int passwordAttemptWindow;
		string passwordStrengthRegularExpression;
		bool enablePasswordReset;
		bool enablePasswordRetrieval;
		bool requiresQuestionAndAnswer;
		bool requiresUniqueEmail;
		int newPasswordLength;
		bool emailAsUserName;

		Models.GalleryContext Database()
		{
			return new Models.GalleryContext();
		}

		public override void Initialize(string name, NameValueCollection config)
		{
			if (string.IsNullOrEmpty(config["description"]))
			{
				config.Remove("description");
				config.Add("description", "Gallery Membership Provider");
			}

			base.Initialize(name, config);

			ApplicationName = GetConfigValue(config["applicationName"], HostingEnvironment.ApplicationVirtualPath);
			maxInvalidPasswordAttempts = Convert.ToInt32(GetConfigValue(config["maxInvalidPasswordAttempts"], "5"));
			passwordAttemptWindow = Convert.ToInt32(GetConfigValue(config["passwordAttemptWindow"], "10"));
			minRequiredNonAlphanumericCharacters = Convert.ToInt32(GetConfigValue(config["minRequiredNonAlphanumericCharacters"], "1"));
			minRequiredPasswordLength = Convert.ToInt32(GetConfigValue(config["minRequiredPasswordLength"], "7"));
			passwordStrengthRegularExpression = Convert.ToString(GetConfigValue(config["passwordStrengthRegularExpression"], ""));
			enablePasswordReset = Convert.ToBoolean(GetConfigValue(config["enablePasswordReset"], "true"));
			enablePasswordRetrieval = Convert.ToBoolean(GetConfigValue(config["enablePasswordRetrieval"], "true"));
			requiresQuestionAndAnswer = Convert.ToBoolean(GetConfigValue(config["requiresQuestionAndAnswer"], "false"));
			requiresUniqueEmail = Convert.ToBoolean(GetConfigValue(config["requiresUniqueEmail"], "true"));
			newPasswordLength = Convert.ToInt32(GetConfigValue(config["newPasswordLength"], "8"));
			emailAsUserName = Convert.ToBoolean(GetConfigValue(config["emailAsUserName"], "false"));

			var formatString = GetConfigValue(config["passwordFormat"], MembershipPasswordFormat.Hashed.ToString());
			if (!Enum.TryParse<MembershipPasswordFormat>(formatString, out passwordFormat))
				throw new ProviderException("Password format not supported.");

			var cfg = WebConfigurationManager.OpenWebConfiguration(HostingEnvironment.ApplicationVirtualPath);
			machineKey = (MachineKeySection)cfg.GetSection("system.web/machineKey");

			if (machineKey.ValidationKey.Contains("AutoGenerate"))
			if (PasswordFormat != MembershipPasswordFormat.Clear)
				throw new ProviderException("Hashed or Encrypted passwords are not supported with auto-generated keys.");
		}

		string GetConfigValue(string configValue, string defaultValue)
		{
			return string.IsNullOrEmpty(configValue) ? defaultValue : configValue;
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
					Email = emailAsUserName ? userName : null,
					Password = EncodePassword(password),
					ConfirmationToken = token,
					IsConfirmed = !requireConfirmation
				};
				if (values != null)
				{
					object val;
					if (values.TryGetValue("Alias", out val) && val is string)
						user.Alias = (string)val;
					if (!emailAsUserName && values.TryGetValue("Email", out val) && val is string)
						user.Email = (string)val;

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
				user.Password = EncodePassword(newPassword);
				user.PasswordChangedDate = DateTime.UtcNow;
				return true;
			}
		}

		public override string ApplicationName { get; set; }

		public override bool ChangePassword(string userName, string oldPassword, string newPassword)
		{
			using (var db = Database())
			{
				var user = db.Users.FirstOrDefault(r => r.UserName == userName);
				if (user == null || !CheckPassword(oldPassword, user.Password))
					return false;
				user.Password = EncodePassword(newPassword);
				user.PasswordChangedDate = DateTime.UtcNow;
				db.SaveChanges();
				return true;
			}
		}

		public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
		{
			using (var db = Database())
			{
				var user = db.Users.FirstOrDefault(r => r.UserName == username);
				if (user != null && CheckPassword(password, user.Password))
				{
					user.PasswordQuestion = newPasswordQuestion;
					user.PasswordAnswer = EncodePassword(newPasswordAnswer);
					return true;
				}
			}
			return false;
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
						Email = emailAsUserName ? username : null,
						Password = EncodePassword(password),
						PasswordQuestion = passwordQuestion,
						PasswordAnswer = EncodePassword(passwordAnswer)
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
			get { return enablePasswordReset; }
		}

		public override bool EnablePasswordRetrieval
		{
			get { return enablePasswordRetrieval; }
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
			get { return maxInvalidPasswordAttempts; }
		}

		public override int MinRequiredNonAlphanumericCharacters
		{
			get { return minRequiredNonAlphanumericCharacters; }
		}

		public override int MinRequiredPasswordLength
		{
			get { return minRequiredPasswordLength; }
		}

		public override int PasswordAttemptWindow
		{
			get { return passwordAttemptWindow; }
		}

		public override MembershipPasswordFormat PasswordFormat
		{
			get { return passwordFormat; }
		}

		public override string PasswordStrengthRegularExpression
		{
			get { return passwordStrengthRegularExpression; }
		}

		public override bool RequiresQuestionAndAnswer
		{
			get { return requiresQuestionAndAnswer; }
		}

		public override bool RequiresUniqueEmail
		{
			get { return requiresUniqueEmail; }
		}

		public override string ResetPassword(string username, string answer)
		{
			using (var db = Database())
			{

				var user = db.Users.FirstOrDefault(r => r.UserName == username && r.IsConfirmed);
				if (user == null)
					throw new MembershipPasswordException("The supplied user name is not found.");
				if (!CheckPassword(answer, user.PasswordAnswer))
					throw new MembershipPasswordException("Incorrect password answer.");
				string newPassword = System.Web.Security.Membership.GeneratePassword(newPasswordLength, MinRequiredNonAlphanumericCharacters);
				user.LastLoginDate = DateTime.UtcNow;
				user.Password = EncodePassword(newPassword);
				db.SaveChanges();
				return newPassword;
			}
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
				var user = db.Users.FirstOrDefault(r => r.UserName == username && r.IsConfirmed);
				if (user == null || !CheckPassword(password, user.Password))
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

		string EncodePassword(string password)
		{
			string encodedPassword = password;

			switch (PasswordFormat)
			{
				case MembershipPasswordFormat.Clear:
					break;
				case MembershipPasswordFormat.Encrypted:
					encodedPassword = Convert.ToBase64String(EncryptPassword(Encoding.Unicode.GetBytes(password)));
					break;
				case MembershipPasswordFormat.Hashed:
					HMACSHA1 hash = new HMACSHA1();
					hash.Key = HexToByte(machineKey.ValidationKey);
					encodedPassword = Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(password)));
					break;
				default:
					throw new ProviderException("Unsupported password format.");
			}

			return encodedPassword;
		}

		string UnEncodePassword(string encodedPassword)
		{
			string password = encodedPassword;

			switch (PasswordFormat)
			{
				case MembershipPasswordFormat.Clear:
					break;
				case MembershipPasswordFormat.Encrypted:
					password = Encoding.Unicode.GetString(DecryptPassword(Convert.FromBase64String(password)));
					break;
				case MembershipPasswordFormat.Hashed:
					throw new ProviderException("Cannot unencode a hashed password.");
				default:
					throw new ProviderException("Unsupported password format.");
			}

			return password;
		}

		byte[] HexToByte(string hexString)
		{
			var returnBytes = new byte[hexString.Length / 2];
			for (int i = 0; i < returnBytes.Length; i++)
				returnBytes[i] = Convert.ToByte(hexString.Substring(i*2, 2), 16);
			return returnBytes;
		}

		bool CheckPassword(string password, string dbpassword)
		{
			string pass1 = password;
			string pass2 = dbpassword;

			switch (PasswordFormat)
			{
				case MembershipPasswordFormat.Encrypted:
					pass2 = UnEncodePassword(dbpassword);
					break;
				case MembershipPasswordFormat.Hashed:
					pass1 = EncodePassword(password);
					break;
			}

			return pass1 == pass2;
		}
	}
}