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
    /// AspNet.Identity RoleStore implemented on N2 ItemBridge
    /// 
    /// <seealso cref="http://msdn.microsoft.com/en-us/library/dn457182(v=vs.111).aspx"> RoleManager </seealso>
    /// </summary>
    /// <remarks> 
    /// N2 classic-membership and AspNet.Identity user roles are stored by ItemBridge.
    /// No data migration is nessesary when moving from classic-membership to Identity.
    /// </remarks>
    [Service]
    public class IdentityRoleStore : IRoleStore<IRole> // todo: IQueryableRoleStore<IRole>
    {
        private RoleManager<IRole> roleManager;

        public IdentityRoleStore(ItemBridge bridge)
        {
            if (bridge == null)
                throw new ArgumentNullException("ItemBridge");
            Bridge = bridge;
        }

        protected ItemBridge Bridge { get; private set; }

        /// <summary> Returns Role manager that operates on the role store </summary>
        public RoleManager<IRole> GetRoleManager() 
        {
            if (roleManager == null)
            {
                // Plain RoleManager is good enough:
                roleManager = new RoleManager<IRole>(this);
            }
            return roleManager;
        }

        private void CloseRoleManager()
        {
            if (roleManager != null)
            {
                roleManager.Dispose();
                roleManager = null;
            }
        }

        #region IDisposable

        public void Dispose()
        {
            CloseRoleManager();
        }

        #endregion

        /// <summary> Simple role class 
        /// <seealso cref="http://msdn.microsoft.com/en-us/library/dn613268(v=vs.108).aspx">IRole</seealso></summary>
        internal class Role : IRole
        {
            public Role(string id, string name) { Id = id; Name = name; }
            /// <summary> Id of the role </summary>
            public string Id   { get; private set; }
            /// <summary> Name of the role </summary>
            public string Name { get; set; }
        }
 
        #region IRoleStore

        /// <summary> Creates a new role </summary>
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
        #region IQueryableRoleStore (todo)

        // IQueryable<IRole> Roles { get; }

        #endregion

        #region private: Roles implemented on ItemBridge

        internal string[] GetAllRoleNames()
        {
            var userList = Bridge.GetUserContainer(true);
            return userList.GetRoleNames();
        }
        internal IEnumerable<IRole> GetAllRoles()
        {
            foreach (string roleName in GetAllRoleNames())
                yield return ToRole(roleName);
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
            Bridge.Save(userList);
            return 0;
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
            Bridge.Save(userList); 
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
