using System;
using System.Web.Security;
using System.Linq;

namespace Pablo.Gallery.Logic.Membership
{
	public class GalleryRoleProvider : RoleProvider
	{
		Models.GalleryContext Database()
		{
			return new Models.GalleryContext();
		}

		public override void AddUsersToRoles(string[] usernames, string[] rolenames)
		{
			throw new NotImplementedException();
		}

		public override void CreateRole(string rolename)
		{
			throw new NotImplementedException();
		}

		public override bool DeleteRole(string rolename, bool throwOnPopulatedRole)
		{
			throw new NotImplementedException();
		}

		public override string[] FindUsersInRole(string roleName, string usernameToMatch)
		{
			throw new NotImplementedException();
		}

		public override string[] GetAllRoles()
		{
			using (var db = Database())
			{
				return db.Roles.Select(r => r.Name).ToArray();
			}
		}

		public override string[] GetRolesForUser(string username)
		{
			using (var db = Database())
			{
				var user = db.Users.FirstOrDefault(r => r.UserName == username);
				if (user == null)
					return new string[0];
				var roles = user.Roles.Select(r => r.Name).ToArray();
				return roles;
			}
		}

		public override string[] GetUsersInRole(string rolename)
		{
			using (var db = Database())
			{
				var users = db.Users.Where(u => u.Roles.Any(r => r.Name == rolename));
				return users.Select(r => r.UserName).ToArray();
			}
		}

		public override bool IsUserInRole(string username, string rolename)
		{
			using (var db = Database())
			{
				return db.Users.Any(u => u.UserName == username && u.Roles.Any(r => r.Name == rolename));
			}
		}

		public override void RemoveUsersFromRoles(string[] usernames, string[] rolenames)
		{
			throw new NotImplementedException();
		}

		public override bool RoleExists(string rolename)
		{
			using (var db = Database())
			{
				return db.Roles.Any(r => r.Name == rolename);
			}
		}

		public override string ApplicationName
		{
			get;
			set;
		}
	}
}

