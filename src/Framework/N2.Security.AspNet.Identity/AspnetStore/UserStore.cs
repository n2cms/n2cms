using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using N2.Security;
using N2.Persistence;
using N2.Engine;

// review: (JH) should we silently upgrade non-TUser persistent data? See also: ToApplicationUser.
// review: (JH) should we define a one-time upgrade procedure?
// review: (JH) N2.Security.ContentMembershipProvider includes additional logics (unique UserName, Email etc).
//              How the logics should be migrated to N2 Aspnet.Identity?
//              Should we attach logics on Items.User to assure logics inplace for all user data changes? 

namespace N2.Security.AspNet.Identity
{
    /// <summary>
    /// AspNet.identity UserStore implemented on N2
    /// </summary>
    /// <remarks>
    /// TUser as user type is registered with Bridge by UserStore, overriding any configuration parameters.
    /// Warning: data migration should be planned when new TUser type is going to be introduced.
    ///          <see cref="ItemBridge.SetUserType"/> Bridge user type rules and limitations.
    /// </remarks>
    [Service]
    public class UserStore<TUser>
        : IUserStore<TUser>, IUserPasswordStore<TUser>, IUserSecurityStampStore<TUser>, IUserLoginStore<TUser>, IUserRoleStore<TUser>
          where TUser : ContentUser
    { 
        public UserStore(ItemBridge bridge)
        {
            if (bridge == null)
                throw new ArgumentNullException("ItemBridge");
            Bridge = bridge;
            Bridge.SetUserType(typeof(TUser));
        }

        protected Logger<UserStore<TUser>> logger;

        public ItemBridge Bridge { get; private set; }

        public UserManager<TUser> GetUserManager() { return new UserManager<TUser>(this); }

        #region IDisposable

        public void Dispose()
        {
            GC.SuppressFinalize(this);
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

        public virtual Task<TUser> FindByIdAsync(string userId)
        {
            return Task.FromResult(BridgeFindByUserId(userId));
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
        #region private: Bridge
        private static string[] UserDefaultRoles = { "Everyone", "Members", "Writers" }; // TODO

        internal bool AddUser(TUser user)
        {
            bool result = SaveUser(user);
            if (result && (UserDefaultRoles.Length > 0))
                AddUserToRoles(user, UserDefaultRoles);
            return result;
        }

        internal bool SaveUser(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            var userList = Bridge.GetUserContainer(true);
            user.Parent = userList;
            Bridge.SaveUser(user); 
            return true;
        }

        internal bool DeleteUser(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            var u = Bridge.GetUser(user.UserName);
            if (u == null)
                return false;
            Bridge.DeleteUser(user); 
            return true;
        }

        private TUser BridgeFindByUserId(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return null;

            var userList = Bridge.GetUserContainer(false);
            if (userList == null)
                return null;

            // TUser: see review question about upgrading old users
            return Bridge.Repository.Find(Parameter.Equal("Parent", userList), /*Parameter.TypeEqual(typeof(TUser)),*/ ContentUser.UserIdQueryParameter(userId))
                .Select(u => ToApplicationUser(u)).Where(u => (u != null))         
                .Take(1).FirstOrDefault();
        }

        internal TUser FindUserByName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return null;
            ContentItem user = Bridge.GetUser(userName);
            return ToApplicationUser(user);
        }

        internal IEnumerable<TUser> GetUsers(int startIndex, int max)
        {
            return Bridge.GetUsers(startIndex, max).Select(u => ToApplicationUser(u)).Where(u => (u != null)); 
        }
        
        private TUser ToApplicationUser(ContentItem user)
        {
            var userT = user as TUser;
            if ((user != null) && (userT == null))
                logger.WarnFormat("Unexpected user type found. Null user returned! Found: {0}, Expected: {1}", user.GetType().AssemblyQualifiedName, typeof(TUser).AssemblyQualifiedName);
            return userT; // will return null when not of required type (should be silently upgraded ?)
        }

        internal int GetUsersCount()
        {
            return Bridge.GetUsersCount();
        }

        internal void UnlockUser(string userName)
        {
            var user = FindUserByName(userName);
            user.IsLockedOut = false;
            SaveUser(user);
        }

        #endregion

        #region IUserPasswordStore

        public virtual Task<string> GetPasswordHashAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            return Task.FromResult<string>(user.PasswordHash);
        }

        public virtual Task<bool> HasPasswordAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            return Task.FromResult<bool>(!string.IsNullOrWhiteSpace(user.PasswordHash));
        }

        public virtual Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            user.PasswordHash = passwordHash;
            // return Task.FromResult<int>(0);
            return Task.FromResult(SaveUser(user));
        }

        #endregion

        #region IUserSecurityStampStore

        public virtual Task<string> GetSecurityStampAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            return Task.FromResult<string>(user.SecurityStamp);
        }

        public virtual Task SetSecurityStampAsync(TUser user, string stamp)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            user.SecurityStamp = stamp; 
            // return Task.FromResult<int>(0);
            return Task.FromResult(SaveUser(user));
        }

        #endregion

        #region IUserLoginStore

        public virtual Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            if (login == null)
                throw new ArgumentNullException("login");
            return Task.FromResult(Bridge.AddUserLogin(user, login.LoginProvider, login.ProviderKey));
        }

        public virtual Task<TUser> FindAsync(UserLoginInfo login)
        {
            if (login == null)
                throw new ArgumentNullException("login");
            return Task.FromResult(BridgeFindByLogin(login.LoginProvider, login.ProviderKey));
        }

        public virtual Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            return Task.FromResult(BridgeFindLogins(user));
        }

        public virtual Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {
            if (login == null)
                throw new ArgumentNullException("login");
            return Task.FromResult(BridgeRemoveLogin(user, login.LoginProvider, login.ProviderKey));
        }

        #endregion
        #region private: Bridge

        private TUser BridgeFindByLogin(string loginProvider, string providerKey)
        {
            var loginItem = Bridge.FindLogin(loginProvider, providerKey);
            return (loginItem != null ? loginItem.Parent as TUser : null);
        }

        private IList<UserLoginInfo> BridgeFindLogins(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            return Bridge.FindUserLogins(user)
                .Select(loginItem => new UserLoginInfo(loginItem.LoginProvider, loginItem.ProviderKey))
                .ToList();
        }
        
        private bool BridgeRemoveLogin(TUser user, string loginProvider, string providerKey)
        {
            return Bridge.DeleteUserLogin(user, loginProvider, providerKey);
        }
        
        #endregion

        #region IUserRoleStore

        public virtual Task AddToRoleAsync(TUser user, string role)
        {
            return Task.FromResult(AddUserToRole(user, role));
        }

        public virtual Task<IList<string>> GetRolesAsync(TUser user)
        {
            return Task.FromResult(BridgeGetUserRoles(user));
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
        #region private: Bridge

        /// <summary> Exist one or more users in specified role? </summary>
        internal bool HasUsersInRole(string roleName)
        {
            return Bridge.HasUsersInRole(roleName);
        }

        /// <summary> Returns users (UserNames) in specified role </summary>
        internal string[] GetUsersInRole(string roleName)
        {
            return Bridge.GetUsersInRole(roleName,int.MaxValue);
        }

        /// <summary> Returns all roles soecified user is in </summary>
        internal string[] GetRolesForUser(string userName)
        {
            TUser user = FindUserByName(userName);
            if (user != null)
                return user.GetRoles();
            else
                return new string[] { };
        }

        internal bool AddUserToRole(TUser user, string role)
        {
            return AddUserToRoles(user, new string[] { role });
        }
        internal bool AddUserToRoles(TUser user, string[] roles)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if ((roles == null) || (roles.Length == 0))
                throw new ArgumentNullException("roles");  

            var userList = Bridge.GetUserContainer(false);
            if (userList == null)
                throw new ArgumentNullException("userList");

            foreach (string role in roles)
            {
                if (string.IsNullOrWhiteSpace(role) || !userList.HasRole(role))
                    throw new ArgumentOutOfRangeException(string.Format("Unknown role {0}.", role));

                if (user.Roles.Contains(role))
                    return false;

                user.Roles.Add(role);
            }
            Bridge.SaveUser(user);
            return true;
        }
        
        private IList<string> BridgeGetUserRoles(TUser user)
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
            if (user == null)
                throw new ArgumentNullException("user");

            if (string.IsNullOrWhiteSpace(role))
                return false;

            if (!user.Roles.Contains(role))
                return false;

            user.Roles.Remove(role);
            Bridge.SaveUser(user);
            return true;
        }

        #endregion

    }
}
