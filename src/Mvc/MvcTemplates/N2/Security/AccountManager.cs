using N2.Engine;
using N2.Plugin;

namespace N2.Security
{
    /// <summary>
    /// User accounts management resources
    /// </summary>
    [Service]
    public class AccountManager : IAutoStart
	{
        public AccountManager()
		{
		}

        public virtual void Start()
        {
            N2.Web.Url.SetToken(LoginPageUrlToken,        "{ManagementUrl}/Login.aspx");
            N2.Web.Url.SetToken(LogoutPageUrlToken,       "{ManagementUrl}/Login.aspx?logout=true");
            N2.Web.Url.SetToken(UsersPageUrlToken,        "{ManagementUrl}/Users/Users.aspx");
            N2.Web.Url.SetToken(UsersEditPageUrlToken,    "{ManagementUrl}/Users/Edit.aspx");
            N2.Web.Url.SetToken(RolesPageUrlToken ,       "{ManagementUrl}/Roles/Roles.aspx");
            N2.Web.Url.SetToken(EditPasswordPageUrlToken, "{ManagementUrl}/Myself/EditPassword.aspx");
            
        }

        public virtual void Stop()
        {
        }

        // Framework\N2\Edit\Api\InterfaceBuilder.cs
        // Mvc\MvcTemplates\N2\ManagementItem.cs
        // MvcTemplates\N2\Top.master
        // Dinamico\Themes\Default\Views\Shared\LayoutPartials\LogOn.cshtml
        public const string LoginPageUrlToken         = "{Account.Login.PageUrl}";
        public const string LogoutPageUrlToken        = "{Account.Logout.PageUrl}";
        public const string UsersPageUrlToken         = "{Account.Users.PageUrl}";
        public const string UsersEditPageUrlToken     = "{Account.Users.Edit.PageUrl}";
        public const string RolesPageUrlToken         = "{Account.Roles.PageUrl}";
        public const string EditPasswordPageUrlToken  = "{Account.EditPassword.PageUrl}";

        // TODO: login dialog as part (external providers live in full pages)
        // Dinamico\Themes\Default\Views\ContentParts\LoginForm.cshtml  (ContentPart)
    }
}
