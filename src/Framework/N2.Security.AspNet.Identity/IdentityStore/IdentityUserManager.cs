using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace N2.Security.AspNet.Identity
{
    /// <summary> Manages user accounts, stored in identityUserStore 
    /// <seealso cref="CompatibilityModePasswordHasher"> You may configure UserManager to support classic-membership password hashes </seealso>
    /// </summary>
    public abstract class IdentityUserManager<TUser> : UserManager<TUser, int>
        where TUser : ContentUser
    {
        public IdentityUserManager(IdentityUserStore<TUser> store)
            : base(store)
        {
        }
    }

}
