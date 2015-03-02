using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web.Security;
using System.Web.UI.WebControls;
using N2.Collections;
using N2.Definitions;
using N2.Details;
using N2.Engine;
using N2.Persistence.Search;
using N2.Integrity;
using N2.Edit;
using N2.Web;
using N2.Management;

namespace N2.Security.Items
{
    [PartDefinition("User List", 
        SortOrder = 2000,
        IconClass = "fa fa-user shadow",
        AuthorizedRoles = new string[0])]
    [WithEditableTitle("Title", 10)]
    [Throwable(AllowInTrash.No)]
    [Indexable(IsIndexable = false)]
    [RestrictParents(typeof(IRootPage))]
    [GroupChildren(GroupChildrenMode.AlphabeticalIndex)]
    [Versionable(AllowVersions.No)]
    public class UserList : ManagementItem, ISystemNode, IInjectable<ISecurityManager>
    {
        ISecurityManager securityManager;

        protected ISecurityManager SecurityManager
        {
            get { return securityManager ?? Context.Current.SecurityManager; }
        }

        [EditableText("Roles", 100, TextMode = TextBoxMode.MultiLine, Rows = 10, RequiredPermission = Permission.Administer)]
        public virtual string Roles
        {
            get { return GetDetail("Roles", "Everyone"); }
            set { SetDetail("Roles", value, "Everyone"); }
        }

        /// <summary>List of all known roles </summary>
        public virtual string[] GetRoleNames()
        {
            return Roles.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>Is role already present? </summary>
        /// <param name="roleName">The role</param>
        public virtual bool HasRole(string roleName)
        {
            return (Array.IndexOf<string>(GetRoleNames(), roleName) >= 0);
        }

        /// <summary>Adds a role if not already present.</summary>
        /// <param name="roleName">The role to add.</param>
        public virtual void AddRole(string roleName)
        {
            if (!HasRole(roleName))
                Roles += Environment.NewLine + roleName;
        }

        /// <summary>Removes a role if existing.</summary>
        /// <param name="roleName">The role to remove.</param>
        public virtual void RemoveRole(string roleName)
        {
            List<string> roles = new List<string>();
            foreach (string existingRole in GetRoleNames())
            {
                if (!existingRole.Equals(roleName))
                {
                    roles.Add(existingRole);
                }
            }
            Roles = string.Join(Environment.NewLine, roles.ToArray());
        }

        public virtual MembershipUserCollection GetMembershipUsers(string providerName)
        {
            MembershipUserCollection muc = new MembershipUserCollection();
            foreach (User u in Children.FindRange(0, 100000))
            {
                muc.Add(u.GetMembershipUser(providerName));
            }
            return muc;
        }

        public virtual MembershipUserCollection GetMembershipUsers(string providerName, int startIndex, int maxResults,
                                                                   out int totalRecords)
        {
            MembershipUserCollection muc = new MembershipUserCollection();
            foreach (User u in Children.FindRange(startIndex, maxResults))
            {
                    muc.Add(u.GetMembershipUser(providerName));
            }
            totalRecords = Children.Count;
            return muc;
        }

        public override bool IsAuthorized(IPrincipal user)
        {
            return base.IsAuthorized(user) && SecurityManager.IsAdmin(user);
        }

        #region IInjectable<ISecurityManager> Members

        void IInjectable<ISecurityManager>.Set(ISecurityManager securityManager)
        {
            this.securityManager = securityManager;
        }

        #endregion
    }
}
