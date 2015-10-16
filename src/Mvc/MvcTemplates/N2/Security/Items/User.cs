using System;
using System.Security.Principal;
using System.Web.Security;
using N2.Definitions;
using N2.Details;
using N2.Integrity;
using N2.Security.Details;
using N2.Engine;
using N2.Edit;
using N2.Web;
using N2.Management;

namespace N2.Security.Items
{
    [PageDefinition("User", IconClass = "fa fa-user", InstallerVisibility = N2.Installation.InstallerHint.NeverRootOrStartPage)]
    [RestrictParents(typeof (UserList))]
    [Throwable(AllowInTrash.No)]
    [Versionable(AllowVersions.No)]
    public class User : ManagementItem, IInjectable<ISecurityManager>
    {
        private ISecurityManager security;

        [EditableText("Title", 10, Required = true, LocalizationClassKey = "Displayname")]
        public override string Title
        {
            get { return base.Title; }
            set { base.Title = value; }
        }

        [EditableText("Username", 20, Required = true, LocalizationClassKey = "Username")]
        public override string Name
        {
            get { return base.Name; }
            set { base.Name = value; }
        }

        [EditablePassword("Password", 30, IsIndexable = false, Placeholder = "****")]
        public virtual string Password
        {
            get { return GetDetail("Password", string.Empty); }
            set { SetDetail("Password", value, string.Empty); }
        }

        [EditableText("Email", 40, Validate = true, ValidationExpression = "[^@]+@[^@.]+[.][^@.]+")]
        public virtual string Email
        {
            get { return GetDetail("Email", string.Empty); }
            set { SetDetail("Email", value, string.Empty); }
        }

        [EditableRoles(Title = "Roles", SortOrder = 50)]
        public virtual DetailCollection Roles
        {
            get { return DetailCollections["Roles"]; }
        }

        [EditableText("PasswordQuestion", 120)]
        public virtual string PasswordQuestion
        {
            get { return GetDetail("PasswordQuestion", string.Empty); }
            set { SetDetail("PasswordQuestion", value, string.Empty); }
        }

        [EditableText("PasswordAnswer", 130, IsIndexable = false)]
        public virtual string PasswordAnswer
        {
            get { return GetDetail("PasswordAnswer", string.Empty); }
            set { SetDetail("PasswordAnswer", value, string.Empty); }
        }

        [EditableCheckBox("Is Online", 140)]
        public virtual bool IsOnline
        {
            get { return GetDetail("IsOnline", false); }
            set { SetDetail("IsOnline", value, false); }
        }

        [EditableCheckBox("Is Approved", 142)]
        public virtual bool IsApproved
        {
            get { return GetDetail("IsApproved", false); }
            set { SetDetail("IsApproved", value, false); }
        }

        [EditableCheckBox("Is Locked Out", 144)]
        public virtual bool IsLockedOut
        {
            get { return GetDetail("IsLockedOut", false); }
            set { SetDetail("IsLockedOut", value, false); }
        }

        [EditableCheckBox("Is Login", 146)]
        public virtual bool IsLogin
        {
            get { return GetDetail("IsLogin", true); }
            set { SetDetail("IsLogin", value, true); }
        }

        [EditableCheckBox("Is Profile", 148)]
        public virtual bool IsProfile
        {
            get { return GetDetail("IsProfile", false); }
            set { SetDetail("IsProfile", value, false); }
        }

        [EditableText("Comment", 150)]
        public virtual string Comment
        {
            get { return GetDetail("Comment", string.Empty); }
            set { SetDetail("Comment", value, string.Empty); }
        }

        [EditableDate("Last Login Date", 160)]
        public virtual DateTime LastLoginDate
        {
            get { return (DateTime) (GetDetail("LastLoginDate") ?? Published.Value); }
            set { SetDetail("LastLoginDate", value, Published.Value); }
        }

        [EditableDate("Last Activity Date", 162)]
        public virtual DateTime LastActivityDate
        {
            get { return (DateTime) (GetDetail("LastActivityDate") ?? Published.Value); }
            set { SetDetail("LastActivityDate", value, Published.Value); }
        }

        [EditableDate("Last Password Changed Date", 164)]
        public virtual DateTime LastPasswordChangedDate
        {
            get { return (DateTime) (GetDetail("LastPasswordChangedDate") ?? Published.Value); }
            set { SetDetail("LastPasswordChangedDate", value, Published.Value); }
        }

        [EditableDate("Last Lockout Date", 166)]
        public virtual DateTime? LastLockoutDate
        {
            get { return (DateTime?) GetDetail("LastLockoutDate"); }
            set { SetDetail("LastLockoutDate", value < new DateTime(2000, 1, 1) ? null : value); }
        }

        /// <summary>This property is always false because a User is never a Page.</summary>
        public override bool IsPage
        {
            get { return false; }
        }

        public virtual MembershipUser GetMembershipUser(string providerName)
        {
            return
                new MembershipUser(providerName, Name, new Guid(ID, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0), Email, PasswordQuestion, Comment, IsApproved, IsLockedOut,
                                   Created, LastLoginDate, LastActivityDate, LastPasswordChangedDate,
                                   (LastLockoutDate ?? DateTime.MinValue));
        }

        public virtual void UpdateFromMembershipUser(MembershipUser mu)
        {
            Comment = mu.Comment;
            Created = mu.CreationDate;
            Email = mu.Email;
            IsApproved = mu.IsApproved;
            IsLockedOut = mu.IsLockedOut;
            IsOnline = mu.IsOnline;
            LastActivityDate = mu.LastActivityDate;
            LastLockoutDate = mu.LastLockoutDate;
            LastLoginDate = mu.LastLoginDate;
            LastPasswordChangedDate = mu.LastPasswordChangedDate;
            PasswordQuestion = mu.PasswordQuestion;
            Name = mu.UserName;
        }

        public virtual string[] GetRoles()
        {
            string[] roles = new string[Roles.Count];
            for (int i = 0; i < roles.Length; i++)
            {
                roles[i] = Roles[i] as string;
            }
            return roles;
        }

        /// <summary>Determines whether a given user is allowed to inspect this User object.</summary>
        /// <param name="user">The user requesitng access to the current instance of a User object.</param>
        /// <returns>True if access should be granted.</returns>
        public override bool IsAuthorized(IPrincipal user)
        {
            return base.IsAuthorized(user) && (security ?? Context.SecurityManager).IsAdmin(user);
        }

        void IInjectable<ISecurityManager>.Set(ISecurityManager dependency)
        {
            this.security = dependency;
        }
    }
}
