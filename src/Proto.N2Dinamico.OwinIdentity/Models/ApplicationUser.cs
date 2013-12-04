using N2;
using N2.Definitions;
using N2.Integrity;

// review: (JH) ApplicationUser vs. N2.Configuration.MembershipElement.UserType
//              Should ApplicationUser be registered with ItemBridge? 
//              A: Yes! It's already implemented. See AspNetAccountManager.

namespace Proto.OwinIdentity.Models
{
    [PageDefinition("ApplicationUser", IconClass = "n2-icon-user")]
    [RestrictParents(typeof(N2.Security.Items.UserList))]
    [Throwable(AllowInTrash.No)]
    [Versionable(AllowVersions.No)]
    public class ApplicationUser : N2.Security.AspNet.Identity.ContentUser
    {
    }
}