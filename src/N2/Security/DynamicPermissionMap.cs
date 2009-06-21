using System.Security.Principal;
using N2.Details;
using System.Collections.Generic;
using System;

namespace N2.Security
{
	/// <summary>
	/// Maps a permission to a set of users and roles. Supports authorizing
	/// users against supplied content items. 
	/// </summary>
	public class DynamicPermissionMap : PermissionMap
	{
		const int MaxPermission = (int)Permission.Administer;
		public const string AuthorizedRolesPrefix = "AuthorizedRoles_";


		
		public DynamicPermissionMap()
		{
		}

		public DynamicPermissionMap(Permission permissionType, string[] roles, string[] users)
			:base(permissionType, roles, users)
		{
		}



		public override bool Authorizes(IPrincipal user, ContentItem item, Permission permission)
		{
			if(permission == Permission.None)
				return true;
			if (!MapsTo(permission))
				return false;

			bool isContentAuthorized = false;

			foreach(Permission permissionLevel in SplitPermission(permission))
			{
				if(!MapsTo(permissionLevel))
					continue;

				if(permissionLevel == Permission.Read)
				{
					if(!item.IsAuthorized(user))
						return false;
					
					isContentAuthorized = true;
					continue;
				}

				DetailCollection details = item.GetDetailCollection(AuthorizedRolesPrefix + permissionLevel, false);
				if(details != null)
				{
					string[] rolesAuthorizedByItem = details.ToArray<string>();
					if (!IsInRoles(user, rolesAuthorizedByItem))
						return false;

					isContentAuthorized = true;
				}
			}

			return isContentAuthorized || base.Authorizes(user, item, permission);
		}



		public static void SetRoles(ContentItem item, Permission permission, params string[] roles)
		{
			foreach (Permission permissionLevel in SplitPermission(permission))
			{
				if(permissionLevel == Permission.Read)
				{
					for (int i = item.AuthorizedRoles.Count - 1; i >= 0; i--)
					{
						AuthorizedRole role = item.AuthorizedRoles[i];
						if(Array.IndexOf(roles, role.Role) < 0)
							item.AuthorizedRoles.RemoveAt(i);
					}
					foreach(string role in roles)
					{
						AuthorizedRole temp = new AuthorizedRole(item, role);
						if(!item.AuthorizedRoles.Contains(temp))
							item.AuthorizedRoles.Add(temp);
					}
				}
				else
				{
					DetailCollection details = item.GetDetailCollection(AuthorizedRolesPrefix + permissionLevel, true);
					details.Replace(roles);
				}
			}
		}

		public static bool IsPermitted(string role, ContentItem item, Permission permission)
		{
			return IsAllRoles(item, permission)
			       || new List<string>(GetRoles(item, permission)).Contains(role);
		}

		public static void SetAllRoles(ContentItem item, Permission permission)
		{
			foreach (Permission permissionLevel in SplitPermission(permission))
			{
				if (permissionLevel == Permission.Read)
				{
					item.AuthorizedRoles.Clear();
					continue;
				}

				DetailCollection details = item.GetDetailCollection(AuthorizedRolesPrefix + permissionLevel, false);
				if (details != null)
					item.DetailCollections.Remove(details.Name);
			}
		}

		public static bool IsAllRoles(ContentItem item, Permission permission)
		{
			if(permission == Permission.Read)
				return item.AuthorizedRoles == null || item.AuthorizedRoles.Count == 0;
			return GetRoles(item, permission) == null;
		}

		private static IEnumerable<string> GetRoles(ContentItem item, Permission permission)
		{
			List<string> roles = null;
			foreach (Permission permissionLevel in SplitPermission(permission))
			{
				if(permissionLevel == Permission.Read)
				{
					foreach(AuthorizedRole role in item.AuthorizedRoles)
					{
						AddTo(ref roles, role.Role);
					}
					continue;
				}

				DetailCollection roleDetails = item.GetDetailCollection(AuthorizedRolesPrefix + permissionLevel, false);
				if (roleDetails == null)
					continue;

				foreach(string role in roleDetails)
				{
					roles = AddTo(ref roles, role);
				}
			}
			return roles;
		}

		private static List<string> AddTo(ref List<string> roles, string role)
		{
			if (roles == null)
				roles = new List<string>();
			if (!roles.Contains(role))
				roles.Add(role);
			return roles;
		}

		protected static IEnumerable<Permission> SplitPermission(Permission permission)
		{
			for (int level = MaxPermission; level > 0; level /= 2)
			{
				Permission permissionLevel = (Permission)level;
				if ((permissionLevel & permission) == permissionLevel)
					yield return permissionLevel;
			}
		}
	}
}