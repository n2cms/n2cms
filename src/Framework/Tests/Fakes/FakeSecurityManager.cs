using System.Collections.Generic;
using System.Security.Principal;
using N2.Collections;
using N2.Security;

namespace N2.Tests.Fakes
{
    public class FakeSecurityManager : ISecurityManager
    {
        #region ISecurityManager Members

        public bool IsEditor(System.Security.Principal.IPrincipal principal)
        {
            return principal != null && principal.Identity.Name == "Editor" || principal.IsInRole("Editors");
        }

        public bool IsAdmin(System.Security.Principal.IPrincipal principal)
        {
            return principal != null && string.Equals(principal.Identity.Name, "Admin", System.StringComparison.InvariantCultureIgnoreCase) || principal.IsInRole("Administrators");
        }

        public bool IsAuthorized(ContentItem item, System.Security.Principal.IPrincipal user)
        {
            if (user == null)
                return item["Unaccessible"] == null;
            return item.Name == user.Identity.Name;
        }

        public bool IsPublished(ContentItem item)
        {
            return PublishedFilter.IsPublished(item);
        }

        public bool Enabled { get; set; }

        public bool ScopeEnabled { get; set; }

        public bool IsAuthorized(System.Security.Principal.IPrincipal user, IEnumerable<string> roles)
        {
            foreach(string role in roles)
                if(user.IsInRole(role))
                    return true;
            return false;
        }

        public bool IsAuthorized(IPrincipal user, Permission permission)
        {
            return user.Identity.Name == permission.ToString();
        }

        public bool IsAuthorized(IPrincipal principal, ContentItem item, Permission permission)
        {
            if (principal == null)
                return item["Unaccessible" + permission] == null;
            return IsAdmin(principal) || item.Name == principal.Identity.Name;
        }

        public void CopyPermissions(ContentItem source, ContentItem destination)
        {
        }

        public Permission GetPermissions(IPrincipal user, ContentItem item)
        {
            return item["Permission"] as Permission? ?? Permission.None;
        }

        public ItemFilter GetAuthorizationFilter(Permission permission)
        {
            return new NullFilter();
        }

        #endregion
    }
}
