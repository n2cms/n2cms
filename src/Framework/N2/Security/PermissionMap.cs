using System;
using System.Collections.Generic;
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
        public bool IsAltered { get; set; }

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
            
            return IsInUsers(user.Identity.Name) || IsInRoles(user, Roles);
        }

        public virtual bool MapsTo(Permission permission)
        {
            return (permission & Permissions) == permission;
        }

        public virtual bool Authorizes(IPrincipal user, ContentItem item, Permission permission)
        {
            if(item != null && permission == Permission.Read && !item.IsAuthorized(user))
                return false;

            return MapsTo(permission) && Contains(user);
        }

        protected bool IsInUsers(string userName)
        {
            if (userName == null)
                return false;

            foreach (string name in Users)
                if (userName.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            return false;
        }

        /// <summary>Asks the user if it is in any of the roles.</summary>
        /// <param name="user">The user to check.</param>
        /// <param name="roles">The roles to look for.</param>
        /// <returns>True if the user is in any of the given roles or the roles are null.</returns>
        public static bool IsInRoles(IPrincipal user, IEnumerable<string> roles)
        {
            if (roles == null) return true;

            foreach (string role in roles)
                if (user.IsInRole(role))
                    return true;
            return false;
        }

        private static Permission[] Levels = { Permission.Administer, Permission.Publish, Permission.Write, Permission.Read, Permission.None };
        public static Permission GetMaximumPermission(Permission permissionFlags)
        {
            foreach (var level in Levels)
            {
                if ((level & permissionFlags) == level)
                    return level;
            }
            return Permission.None;
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
