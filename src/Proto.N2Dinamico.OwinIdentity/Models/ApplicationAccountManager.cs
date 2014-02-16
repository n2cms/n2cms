using N2.Engine;
using N2.Security;
using N2.Security.AspNet.Identity;

namespace Proto.OwinIdentity.Models
{
    [Service(typeof(AccountManager), Replaces = typeof(MembershipAccountManager))]
    public class ApplicationAccountManager : AspNetAccountManager<ApplicationUser> 
    {
        public ApplicationAccountManager(UserStore<ApplicationUser> userStore, RoleStore roleStore)
            : base(userStore,roleStore)
        {
        }
    }
}