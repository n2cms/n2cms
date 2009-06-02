using System;
using System.Security.Principal;

namespace N2.Security
{
	/// <summary>
	/// Maps a permission to a set of users and roles.
	/// </summary>
	public class PermissionMap : ICloneable
	{
		public Permission Permissions { get; set; }
		public string[] Roles { get; set; }
		public string[] Users { get; set; }

		public PermissionMap()
		{
			Permissions = Permission.None;
			Roles = new string[0];
			Users = new string[0];
		}

		public PermissionMap(Permission permissionType, string[] roles, string[] users)
		{
			Permissions = permissionType;
			Roles = roles;
			Users = users;
		}

		public virtual bool Contains(IPrincipal user)
		{
			if (user == null)
				return false;
			
			return IsInUsers(user.Identity.Name) || IsInRoles(user);
		}

		public virtual bool MapsTo(Permission permission)
		{
			return (permission & Permissions) == permission;
		}

		public virtual bool Authorizes(IPrincipal user, ContentItem item, Permission permission)
		{
			return MapsTo(permission) && Contains(user) && item.IsAuthorized(user);
		}

		private bool IsInUsers(string userName)
		{
			foreach (string name in Users)
				if (userName.Equals(name, StringComparison.InvariantCultureIgnoreCase))
					return true;
			return false;
		}

		private bool IsInRoles(IPrincipal user)
		{
			foreach (string role in Roles)
				if (user.IsInRole(role))
					return true;
			return false;
		}

		#region ICloneable Members

		object ICloneable.Clone()
		{
			throw new NotImplementedException();
		}

		public virtual PermissionMap Clone()
		{
			return (PermissionMap) MemberwiseClone();
		}

		#endregion

		#region ToString
		public override string ToString()
		{
			return Permissions + ": roles={" + string.Join(",", Roles) + "} users={" + string.Join(",", Users) + "}";
		}
		#endregion	
	}
}