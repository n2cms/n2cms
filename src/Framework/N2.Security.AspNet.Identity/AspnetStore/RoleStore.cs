using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using N2.Security;
using N2.Persistence;
using N2.Engine;

namespace N2.Security.AspNet.Identity
{
    /// <summary>
    /// AspNet.identity RoleStore implemented on N2
    /// <para>
    /// RoleManager <seealso cref="http://msdn.microsoft.com/en-us/library/dn457182(v=vs.111).aspx"/>
    /// </para>
    /// </summary>
    [Service]
    public class RoleStore : IRoleStore<IRole>
    { 
        public RoleStore(ItemBridge bridge)
        {
            if (bridge == null)
                throw new ArgumentNullException("ItemBridge");
            Bridge = bridge;
        }

        protected ItemBridge Bridge { get; private set; }

        public RoleManager<IRole> GetRoleManager() { return new RoleManager<IRole>(this); }

        #region IDisposable

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #endregion

        private class Role : IRole
        {
            public Role(string id, string name) { Id = id; Name = name; }
            /// <summary> Id of the role </summary>
            public string Id   { get; private set; }
            public string Name { get; set; }
        }
 
        #region IRoleStore

        public virtual Task CreateAsync(IRole role)
        {
            return Task.FromResult(AddRole(role));
        }
        public virtual Task DeleteAsync(IRole role)
        {
            return Task.FromResult(DeleteRole(role));
        }
        public virtual Task<IRole> FindByIdAsync(string roleId)
        {
            return Task.FromResult(FindByRoleId(roleId));
        }
        public virtual Task<IRole> FindByNameAsync(string roleName)
        {
            return Task.FromResult(FindByRoleName(roleName));
        }
        public virtual Task UpdateAsync(IRole role)
        {
            return Task.FromResult(0); // there's no role properties to update
        }
   
        #endregion
        #region private: Bridge

        internal string[] GetAllRoles()
        {
            var userList = Bridge.GetUserContainer(false);
            return userList.GetRoleNames();
        }

        private int AddRole(IRole role) 
        {
            if (role == null)
                throw new ArgumentNullException("role");
            return AddRole(role.Id); 
        }

        internal int AddRole(string roleId)
        {
            var userList = Bridge.GetUserContainer(true);
            userList.AddRole(roleId);
            Bridge.SaveUserContainer(userList);
            return userList.ID;
        }

        private bool DeleteRole(IRole role) 
        {
            if (role == null)
                throw new ArgumentNullException("role");
            return DeleteRole(role.Id);
        }

        internal bool DeleteRole(string roleId)
        {
            var userList = Bridge.GetUserContainer(false);
            if (!userList.HasRole(roleId))
                return false;
            userList.RemoveRole(roleId);
            Bridge.SaveUserContainer(userList); 
            return true;
        }

        private IRole FindByRoleId(string roleId)
        {
            var userList = Bridge.GetUserContainer(false);
            if (string.IsNullOrWhiteSpace(roleId) || (userList == null) || !userList.HasRole(roleId))
                return null;
            return ToRole(roleId);
        }

        private IRole FindByRoleName(string roleName)
        {
            return FindByRoleId(roleName); // implementation internals: roleID == roleName
        }

        private Role ToRole(string roleId)
        {
            return new Role(roleId, roleId); // implementation internals: roleID == roleName
        }

        #endregion
    }
}
