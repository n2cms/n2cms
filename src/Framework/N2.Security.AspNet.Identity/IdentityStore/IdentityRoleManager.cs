using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace N2.Security.AspNet.Identity
{
    /// <summary> Manages user roles, stored in IdentityRoleStore </summary>
    public abstract class IdentityRoleManager : RoleManager<IRole>
    {
        public IdentityRoleManager(IdentityRoleStore store)
            : base(store)
        {
        }
    }

}
