using N2.Engine;

namespace N2.Security
{
    /// <summary>
    /// User accounts management resources (classic membership)
    /// </summary>
    [Service(typeof(AccountResources))]
    public class MembershipAccountResources : AccountResources
    {
        public override void Start()
        {
            base.Start();

            N2.Web.Url.SetToken(LoginPageUrlToken,          "{ManagementUrl}/Login.aspx");
            string logoutUrl = System.Configuration.ConfigurationManager.AppSettings["N2.logout.url"];
            if (!string.IsNullOrEmpty(logoutUrl))
                N2.Web.Url.SetToken(LogoutPageUrlToken, logoutUrl);
            else 
                N2.Web.Url.SetToken(LogoutPageUrlToken,         "{ManagementUrl}/Login.aspx?logout=true");

            N2.Web.Url.SetToken(ManageUserPageUrlToken,     "{ManagementUrl}/Myself/EditPassword.aspx");
            N2.Web.Url.SetToken(EditPasswordPageUrlToken,   "{ManagementUrl}/Myself/EditPassword.aspx");

            N2.Web.Url.SetToken(UsersPageUrlToken,          "{ManagementUrl}/Users/Users.aspx");
            N2.Web.Url.SetToken(UsersEditPageUrlToken,      "{ManagementUrl}/Users/Edit.aspx");

            N2.Web.Url.SetToken(RolesPageUrlToken,          "{ManagementUrl}/Roles/Roles.aspx");

        }
    }
}