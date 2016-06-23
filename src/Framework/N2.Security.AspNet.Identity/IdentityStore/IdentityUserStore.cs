using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using N2.Security;
using N2.Persistence;
using N2.Engine;

// review: (JH) N2 documentation should receive an introduction to ItemBridge User class customization,
//              with description how to upgrade existing user records 
//              after extending User class.

// review: (JH) N2.Security.ContentMembershipProvider includes additional logics (unique UserName, Email etc).
//              How the logics should be migrated to N2 Aspnet.Identity?
//              Should we attach logics on Items.User to assure logics inplace for all user data changes? 

namespace N2.Security.AspNet.Identity
{
    /// <summary>
    /// AspNet.Identity UserStore implemented on N2 ItemBridge
    /// <seealso cref="http://msdn.microsoft.com/en-us/library/microsoft.aspnet.identity(v=vs.108).aspx"> Microsoft.AspNet.Identity Namespace </seealso>
    /// </summary>
    /// <remarks>
    /// TUser user type is registered with Bridge by UserStore, overriding any N2 configuration parameters.
    /// 
    /// Note: IdentityUserStore implementation is designed to be thread safe,
    ///       therefore a service may serve all requests.
    /// </remarks>
    [Service]
    public class IdentityUserStore<TUser>
        : IUserStore<TUser, int>, // todo: IQueryableUserStore<TUser,int> 
          IUserPasswordStore<TUser, int>, IUserSecurityStampStore<TUser, int>, 
          IUserLoginStore<TUser, int>, IUserRoleStore<TUser, int>, // todo: IUserClaimStore<TUser, int>
          IUserEmailStore<TUser, int>, IUserLockoutStore<TUser, int>, IUserPhoneNumberStore<TUser,int>
          where TUser : ContentUser
    {
        private readonly N2.Engine.Logger<IdentityUserStore<TUser>> logger;

        public IdentityUserStore(ItemBridge bridge)
        {
            if (bridge == null)
                throw new ArgumentNullException("ItemBridge");
            Bridge = bridge;
            Bridge.SetUserType(typeof(TUser));
        }

        public ItemBridge Bridge { get; private set; }

        #region IDisposable

        public void Dispose()
        { // Nothing to dispose: UserStore is pure functional class
        }

        #endregion

        #region IUserStore

        public virtual Task CreateAsync(TUser user)
        {
            return Task.FromResult(AddUser(user));
        }

        public virtual Task DeleteAsync(TUser user)
        {
            return Task.FromResult(DeleteUser(user));
        }

        public virtual Task<TUser> FindByIdAsync(int userId)
        {
            return Task.FromResult(FindUserByUserId(userId));
        }

        public virtual Task<TUser> FindByNameAsync(string userName)
        {
            return Task.FromResult(FindUserByName(userName));
        }

        public virtual Task UpdateAsync(TUser user)
        {
            return Task.FromResult(SaveUser(user));
        }

        #endregion
        #region IQueryableUserStore (todo)

        // TODO: public IQueryable<TUser> Users { get; }

        #endregion
        #region private: Users implemented on ItemBridge
        private static string[] UserDefaultRoles = { "Everyone", "Members", "Writers" }; // TODO: should defaults be stored somewhere else?

        private bool AddUser(TUser user)
        {
            bool result = SaveUser(user);
            if (result && (UserDefaultRoles.Length > 0))
                AddUserToRoles(user, UserDefaultRoles);
            return result;
        }

        private bool SaveUser(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            var userList = Bridge.GetUserContainer(true);
            user.Parent = userList;
            Bridge.Save(user); 
            return true;
        }

        internal bool DeleteUser(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            var u = Bridge.GetUser(user.UserName);
            if (u == null)
                return false;
            Bridge.Delete(user); 
            return true;
        }

        private TUser FindUserByUserId(int userId)
        {
            var userList = Bridge.GetUserContainer(false);
            if (userList == null)
                return null;

            // TUser: see review questions on upgrading old users
            return Bridge.Repository.Find(Parameter.Equal("Parent", userList), /*Parameter.TypeEqual(typeof(TUser)),*/ ContentUser.UserIdQueryParameter(userId))
                .Select(u => ToApplicationUser(u)).Where(u => (u != null))
                .FirstOrDefault();
        }

        internal TUser FindUserByName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return null;
            ContentItem user = Bridge.GetUser(userName);
            return ToApplicationUser(user);
        }

        private TUser ToApplicationUser(ContentItem user)
        {
            var userT = user as TUser;
            if ((user != null) && (userT == null))
                logger.WarnFormat("Unexpected user type found. Null user returned! Found: {0}, Expected: {1}", user.GetType().AssemblyQualifiedName, typeof(TUser).AssemblyQualifiedName);
            return userT; // will return null when not of required type (should be silently upgraded ?)
        }

        #endregion
        #region internal: Users (extended)

        internal IEnumerable<TUser> GetUsers(int startIndex, int max)
        {
            return Bridge.GetUsers(startIndex, max).Select(u => ToApplicationUser(u)).Where(u => (u != null)); 
        }

        internal int GetUsersCount()
        {
            return Bridge.GetUsersCount();
        }

        #endregion

        #region IUserPasswordStore

        public virtual Task<string> GetPasswordHashAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            return Task.FromResult<string>(GetUserPasswordHash(user));
        }

        public virtual Task<bool> HasPasswordAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            return Task.FromResult<bool>(!string.IsNullOrWhiteSpace(GetUserPasswordHash(user)));
        }

        public virtual Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            return Task.FromResult(SetPasswordHash(user, passwordHash));
        }

        #endregion
        #region UserPassword implemented on ItemBridge

        private string GetUserPasswordHash(TUser user) { return user.PasswordHash; }

        private bool SetPasswordHash(TUser user, string passwordHash)
        {
            return UpdateUser(user, (storedUser) => { user.PasswordHash = storedUser .PasswordHash = passwordHash; });
        }

        #endregion

        #region IUserSecurityStampStore

        public virtual Task<string> GetSecurityStampAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            return Task.FromResult<string>(GetUserSecurityStamp(user));
        }

        public virtual Task SetSecurityStampAsync(TUser user, string stamp)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            return Task.FromResult(SetSecurityStamp(user, stamp));
        }

        #endregion
        #region SecurityStamp implemented on ItemBridge

        private string GetUserSecurityStamp(TUser user) { return user.SecurityStamp; }

        private bool SetSecurityStamp(TUser user, string stamp)
        {
            return UpdateUser(user, (storedUser) => { user.SecurityStamp = storedUser.SecurityStamp = stamp; });
        }

        #endregion

        #region IUserLoginStore

        public virtual Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            if (login == null)
                throw new ArgumentNullException("login");
            return Task.FromResult(AddUserLogin(user, login.LoginProvider, login.ProviderKey));
        }

        public virtual Task<TUser> FindAsync(UserLoginInfo login)
        {
            if (login == null)
                throw new ArgumentNullException("login");
            return Task.FromResult(FindUserByLogin(login.LoginProvider, login.ProviderKey));
        }

        public virtual Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            return Task.FromResult(FindUserLogins(user));
        }

        public virtual Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {
            if (login == null)
                throw new ArgumentNullException("login");
            return Task.FromResult(RemoveUserLogin(user, login.LoginProvider, login.ProviderKey));
        }

        #endregion
        #region private: UserLogins implemented on Bridge

        private bool AddUserLogin(TUser user, string loginProvider, string providerKey)
        {
            return Bridge.AddUserExternalLoginInfo(user, loginProvider, providerKey);
        }

        private TUser FindUserByLogin(string loginProvider, string providerKey)
        {
            var loginItem = Bridge.FindUserExternalLoginInfo(loginProvider, providerKey);
            return (loginItem != null ? loginItem.Parent as TUser : null);
        }

        private IList<UserLoginInfo> FindUserLogins(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            return Bridge.FindUserExternalLoginInfos(user)
                .Select(loginItem => new UserLoginInfo(loginItem.LoginProvider, loginItem.ProviderKey))
                .ToList();
        }
        
        private bool RemoveUserLogin(TUser user, string loginProvider, string providerKey)
        {
            return Bridge.DeleteUserExternalLoginInfo(user, loginProvider, providerKey);
        }
        
        #endregion

        #region IUserRoleStore

        public virtual Task AddToRoleAsync(TUser user, string role)
        {
            return Task.FromResult(AddUserToRole(user, role));
        }

        public virtual Task<IList<string>> GetRolesAsync(TUser user)
        {
            return Task.FromResult(GetUserRoles(user));
        }

        public virtual Task<bool> IsInRoleAsync(TUser user, string role)
        {
            return Task.FromResult(IsUserInRole(user, role));
        }

        public virtual Task RemoveFromRoleAsync(TUser user, string role)
        {
            return Task.FromResult(RemoveUserFromRole(user, role));
        }

        #endregion
        #region private: UserRoles implemented on ItemBridge

        internal bool AddUserToRole(TUser user, string role)
        {
            return AddUserToRoles(user, new string[] { role });
        }
        private bool AddUserToRoles(TUser user, string[] roles)
        {
            return Bridge.AddUserToRoles(user, roles);
        }
        
        private IList<string> GetUserRoles(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            return new List<string>(user.GetRoles());
        }

        internal bool IsUserInRole(TUser user, string role)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (string.IsNullOrWhiteSpace(role))
                return false;

            return user.Roles.Contains(role);
        }

        internal bool RemoveUserFromRole(TUser user, string role)
        {
            return Bridge.RemoveUserFromRole(user, role);
        }

        #endregion
        #region internal: Users and Roles (extended)

        /// <summary> Exist one or more users in specified role? </summary>
        internal bool HasUsersInRole(string roleName)
        {
            return Bridge.HasUsersInRole(roleName);
        }

        /// <summary> Returns users (UserNames) in specified role </summary>
        internal string[] GetUsersInRole(string roleName)
        {
            return Bridge.GetUsersInRole(roleName, int.MaxValue);
        }

        /// <summary> Returns all roles specified user is in <seealso cref="GetUserRoles(TUser)"/></summary>
        internal string[] GetRolesForUser(string userName)
        {
            TUser user = FindUserByName(userName);
            if (user != null)
                return user.GetRoles();
            else
                return new string[] { };
        }

        #endregion
        // Seealso: RoleStore

        #region IUserEmailStore

        /// <summary> Returns user associated with the email, null: not found </summary>
        public Task<TUser> FindByEmailAsync(string email)
        {
            return Task.FromResult(FindUserByEmail(email));
        }

        public Task<string> GetEmailAsync(TUser user)
        {
            return Task.FromResult(GetUserEmail(user));
        }

        public Task<bool> GetEmailConfirmedAsync(TUser user)
        {
            return Task.FromResult(IsUserEmailConfirmed(user)); 
        }

        public Task SetEmailAsync(TUser user, string email)
        {
            return Task.FromResult(SetUserEmail(user, email)); 
        }

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed)
        {
            return Task.FromResult(SetUserEmailConfirmed(user, confirmed)); 
        }

        #endregion
        #region Email implemented on ItemBridge

        private TUser FindUserByEmail(string email)
        {
            return Bridge.FindUserByEmail(email) as TUser;
        }

        private string GetUserEmail(TUser user) { return user.Email; }

        internal bool SetUserEmail(TUser user, string email)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            lock(Bridge)
            {
                var theUser = Bridge.GetUser(user.UserName) as ContentUser;
                if (theUser == null)
                    return false;
                theUser.Email = user.Email = email;
                Bridge.Save(theUser);
                return true;
            }
        }

        // review: IsApproved semantics is questionable. Should we introduce a new property IsEmailConfirmed? 
        private bool IsUserEmailConfirmed(TUser user) { return user.IsApproved; }

        private bool SetUserEmailConfirmed(TUser user, bool confirmed)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            lock (Bridge)
            {
                var theUser = Bridge.GetUser(user.UserName) as ContentUser;
                if (theUser == null)
                    return false;
                theUser.IsApproved = user.IsApproved = confirmed;
                Bridge.Save(theUser);
                return true;
            }
        }

        #endregion

        #region IUserLockoutStore

        /// <summary> Returns whether the user can be locked out </summary>
        /// <remarks> It's a flag saying that user can (or can not) be locked in principle.
        ///  It is an opt-in flag for locking. 
        ///  To check if user is locked-out see UserManager.IsLockedOutAsync(user.Id). 
        /// </remarks>
        public Task<bool> GetLockoutEnabledAsync(TUser user)
        {
            // admin cannot be locked out
            return Task.FromResult(GetLockoutEnabled(user));
        }

        /// <summary>  Sets whether the user can be locked out. </summary>
        public Task SetLockoutEnabledAsync(TUser user, bool enabled)
        {
            // Not supported yet
            return Task.FromResult(false);
        }

        /// <summary> Returns the DateTimeOffset that represents the end of a user's lockout, any time in the past should be considered not locked out </summary>
        public Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
        {
            // Note: The functionallity is similar to (but different from) classic membership IsLockedOut, LastLockoutDate
            return Task.FromResult(GetLockoutEndDate(user)); 
        }

        /// <summary> Locks a user out until the specified end date (set to a past date, to unlock a user). </summary>
        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
        {
            return Task.FromResult(SetLockoutEndDate(user, lockoutEnd));
        }

        /// <summary> Returns the current number of failed access attempts. This number usually will be reset whenever the password is verified or the account is locked out. </summary>
        public Task<int> GetAccessFailedCountAsync(TUser user)
        {
            return Task.FromResult(GetAccessFailedCount(user));
        }

        /// <summary> Incremented on an attempt to access the user has failed </summary>
        public Task<int> IncrementAccessFailedCountAsync(TUser user)
        {
            return Task.FromResult(UpdateAccessFailedCount(user, false));
        }

        /// <summary> Resets the access failed count, typically after the account is successfully accessed. </summary>
        public Task ResetAccessFailedCountAsync(TUser user)
        {
            return Task.FromResult(UpdateAccessFailedCount(user,true));
        }

        #endregion
        #region Lockout implemented on ItemBridge

        private bool GetLockoutEnabled(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            // admin cannot be locked out
            return Bridge.IsAdmin(user.UserName);
        }

        private DateTimeOffset GetLockoutEndDate(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            // Note: The functionallity is similar to (but different from) classic membership IsLockedOut, LastLockoutDate
            return user.LockedOutEndDate;
        }

        private bool SetLockoutEndDate(TUser user, DateTimeOffset lockoutEnd)
        {
            return UpdateUser(user, (storedUser) => { user.LockedOutEndDate = storedUser.LockedOutEndDate = lockoutEnd; });
        }

        private int GetAccessFailedCount(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            return user.AccessFailedCount;
        }

        private int UpdateAccessFailedCount(TUser user, bool reset)
        {
            UpdateUser(user, (storedUser) => { user.AccessFailedCount = storedUser.AccessFailedCount = reset ? 0 : (1 + Math.Max(0, storedUser.AccessFailedCount)); });
            return user.AccessFailedCount;
        }

        #endregion
        #region

        internal void UnlockUser(string userName)
        {
            var user = FindUserByName(userName);
            if (user == null)
                return;
            UpdateUser(user, (user1) => { 
                // classic membership:
                user1.IsLockedOut = user.IsLockedOut = false;
                // identity lock-out:
                user1.AccessFailedCount = 0;
                user1.LockedOutEndDate = DateTimeOffset.Now.AddSeconds(-1); 
            });
        }

        #endregion

        #region IUserPhoneNumberStore

        public Task<string> GetPhoneNumberAsync(TUser user)
        {
            return Task.FromResult(GetPhoneNumber(user));
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user)
        {
            return Task.FromResult(GetPhoneNumberConfirmed(user));
        }

        public Task SetPhoneNumberAsync(TUser user, string phoneNumber)
        {
            return Task.FromResult(SetPhoneNumber(user, phoneNumber));
        }

        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed)
        {
            return Task.FromResult(SetPhoneNumberConfirmed(user, confirmed));
        }

        #endregion
        #region PhoneNumber implemented on ItemBridge

        private string GetPhoneNumber(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            return user.PhoneNumber;
        }

        private bool GetPhoneNumberConfirmed(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            return user.IsPhoneNumberConfirmed;
        }

        private bool SetPhoneNumber(TUser user, string phoneNumber)
        {
            return UpdateUser(user, (user1) => { user1.PhoneNumber = phoneNumber; });
        }

        private bool SetPhoneNumberConfirmed(TUser user, bool confirmed)
        {
            return UpdateUser(user, (storedUser) => { storedUser.IsPhoneNumberConfirmed = user.IsPhoneNumberConfirmed = confirmed; });
        }
    
        #endregion

        #region support

        /// <summary> Update a User property (make sure no other properties are affected) </summary>
        private bool UpdateUser(TUser user, Action<ContentUser> update)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            lock (Bridge)
            {
                var theUser = Bridge.GetUser(user.UserName) as ContentUser;
                if (theUser == null)
                {
                    update(user);
                    return false;
                }
                update(theUser);
     
                Bridge.Save(theUser);
                return true;
            }
        }

        #endregion

    }
}
