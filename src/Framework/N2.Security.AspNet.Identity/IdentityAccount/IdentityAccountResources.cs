using N2.Engine;

namespace N2.Security.AspNet.Identity
{
    /// <summary>
    /// User accounts management resources (Aspnet.Identity account management)
    /// </summary>
    /// <remarks>
    /// N2 Management pages are used by default,
    /// login/logout/edit password endpoints are directed to AccountController's Login, LogOff and Manage endpoints
    /// (as provided by default Visual Studio MVC project template).
    /// 
    /// Additionaly you should assure HTTP GET LogOff is accepted and granted to anonymous users
    /// (review and fix Visual Studio generated code, that is comment out restriction and add [AllowAnonymous])
    /// 
    /// Class implementation as N2 service will replace default classic-membership service:
    /// <![CDATA[
    ///  [Service(typeof(ApplicationAccountResources), Replaces = typeof(MembershipAccountResources))]
    ///  public class ApplicationAccountResources : IdentityAccountResources
    ///  {
    ///  }
    /// ]]>
    /// </remarks>
    public abstract class IdentityAccountResources : MembershipAccountResources
	{
        public override void Start()
        {
            base.Start();

            // AccountController endpoints, e.g. provided by VisualStudio MVC 5 template:
            N2.Web.Url.SetToken("{Account.Identity}",          "~/Account");

            N2.Web.Url.SetToken(LoginPageUrlToken,             "{Account.Identity}/Login");
            N2.Web.Url.SetToken(LogoutPageUrlToken,            "{Account.Identity}/LogOff");

            N2.Web.Url.SetToken(ManageUserPageUrlToken,        "{Account.Identity}/Manage");  
            N2.Web.Url.SetToken(EditPasswordPageUrlToken,      "{Account.Identity}/Manage");     
       
        }
    }
}
