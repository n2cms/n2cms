using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;  // Membership
using N2.Engine;

namespace N2.Security
{
    /// <summary>
    /// User accounts management methods (clasic membership)
    /// </summary>
    [Service(typeof(AccountManager))]
    public class MembershipAccountManager : AccountManager
    {
        public MembershipAccountManager()
        {
        }

        /// <summary> Returns account type (MEMBERSHIP when a provider is set) </summary>
        public override ManagerType AccountType
        {
            get
            {
                try
                {
                    if (Membership.Providers.Count > 0)
                        return ManagerType.MEMBERSHIP;
                    else
                        return ManagerType.NONE; // no membership providers configured
                }
                catch
                {
                    return ManagerType.NONE; // membership subsystem (probably) not available at all
                }
            }
        }

        #region Users

        public override IAccountInfo FindUserByName(string userName)
        {
            var muser = Membership.GetUser(userName, false);
            return ToAccountInfo(muser);
        }

        public override IList<IAccountInfo> GetUsers(int start, int max)
        {
            int page = start / max;
            int total;
            MembershipUserCollection musers = System.Web.Security.Membership.GetAllUsers(page, max, out total);

            var list = new List<IAccountInfo>();
            foreach (MembershipUser muser in musers)
            {
                list.Add(ToAccountInfo(muser));
            }

            return list;
        }

        private AccountInfo ToAccountInfo(MembershipUser muser)
        {
            return new AccountInfo()
            {
                UserName = muser.UserName,
                Email = muser.Email,
                CreationDate = muser.CreationDate,
                Comment = muser.Comment,
                IsOnline = muser.IsOnline,
                LastLoginDate = muser.LastLoginDate,
                IsLockedOut = muser.IsLockedOut,
                LastLockoutDate = muser.LastLockoutDate,
                IsApproved = muser.IsApproved
            };
        }

        public override int GetUsersCount()
        {
            int total;
            System.Web.Security.Membership.GetAllUsers(0, 1, out total);
            return total;
        }

        public override void UpdateUserEmail(string userName, string email)
        {
            var muser = Membership.GetUser(userName, false);
            if (muser == null)
                throw new N2Exception("User {0} not found", userName);
            muser.Email = email;
            Membership.UpdateUser(muser);
        }

        public override void UnlockUser(string userName)
        {
            var muser = Membership.GetUser(userName, false);
            if (muser == null)
                throw new N2Exception("User {0} not found", userName);
            muser.UnlockUser();
        }

        public override bool ChangePassword(string userName, string newPassword)
        {
            var muser = Membership.GetUser(userName, false);
            if (muser == null)
                throw new N2Exception("User {0} not found", userName);
            string tempPW = muser.ResetPassword();
            bool ok = muser.ChangePassword(tempPW, newPassword);
            return ok;
        }

        public override void DeleteUser(string userName)
        {
            if (userName == null)
                throw new ArgumentNullException("userName");

            System.Web.Security.Membership.DeleteUser(userName, true);
        }

        #endregion

        #region Roles
        // see Mvc\MvcTemplates\N2\Roles

        public override bool AreRolesEnabled()
        {
            return System.Web.Security.Roles.Enabled;
        }

        public override string[] GetAllRoles()
        {
            // review: (JH) Compare logics with GetAvailableRoles - Mvc\MvcTemplates\N2\Content\Security\Default.aspx.cs 

            if (System.Web.Security.Roles.Enabled)
                return Roles.GetAllRoles();
            else
                return new string[] { N2.Security.AuthorizedRole.Everyone };
        }

        public override void CreateRole(string roleName)
        {
            Roles.CreateRole(roleName);
        }

        public override bool DeleteRole(string roleName)
        {
            return Roles.DeleteRole(roleName);
        }

        #endregion

        #region User roles

        public override bool HasUsersInRole(string roleName)
        {
            return Roles.GetUsersInRole(roleName).Any();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            return Roles.GetUsersInRole(roleName);
        }

        public override string[] GetRolesForUser(string userName)
        {
			if (Roles.Enabled)
				return Roles.GetRolesForUser(userName);
			else
				return new string[0];
        }

        public override bool IsUserInRole(string userName, string roleName)
        {
            return Roles.IsUserInRole(userName, roleName);
        }

        public override void AddUserToRole(string userName, string roleName)
        {
            Roles.AddUserToRole(userName, roleName);
        }

        public override void RemoveUserFromRole(string userName, string roleName)
        {
            Roles.RemoveUserFromRole(userName, roleName);
        }

        #endregion

    }
}