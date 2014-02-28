using N2.Engine;

namespace N2.Security.AspNet.Identity
{
    /// <summary>
    /// User accounts management resources (Aspnet.Identity account management)
    /// </summary>
    /// <remarks>
    /// N2 Managament pages are used by default,
    /// login/logout/edit password endpoints are directed to AccountController's Login, LogOff and Manage
    /// (as provided by default Visual Studio MVC project template).
    /// You should assure LogOff is available to GET requests and even to anonymous users
    /// (review and fix Visual Studio generated code, that is comment out restriction and add [AllowAnonymous])
    /// </remarks>
    [Service(typeof(AccountResources), Replaces = typeof(MembershipAccountResources))]
    public class AspNetAccountResources : MembershipAccountResources
	{
        public override void Start()
        {
            base.Start();

            // N2.Web.Url.SetToken("{Account.Echo}",   "~/Home/Echo");

            // AccountController endpoints, e.g. provided by VisualStudio MVC 5 template:
            N2.Web.Url.SetToken("{Account.Aspnet}", "~/Account");

            N2.Web.Url.SetToken(LoginPageUrlToken,        "{Account.Aspnet}/Login");
            N2.Web.Url.SetToken(LogoutPageUrlToken,       "{Account.Aspnet}/LogOff");

            N2.Web.Url.SetToken(EditPasswordPageUrlToken, "{Account.Aspnet}/Manage");

            // Standard N2 Management UI can manage users and roles:
            // N2.Web.Url.SetToken(UsersPageUrlToken,        "{Account.Echo}/Users");
            // N2.Web.Url.SetToken(UsersEditPageUrlToken,    "{Account.Echo}/UsersEdit");          
            // N2.Web.Url.SetToken(RolesPageUrlToken,        "{Account.Echo}/Roles");
            
        }
    }
}
