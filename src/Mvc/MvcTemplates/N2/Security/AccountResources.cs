using System;
using System.Collections.Generic;
using System.Linq;
using N2.Engine;
using N2.Plugin;

namespace N2.Security
{
    /// <summary>
    /// User accounts management resources
    /// </summary>
    public abstract class AccountResources : IAutoStart
	{
        public AccountResources()
		{
		}

        public virtual void Start()
        {            
        }

        public virtual void Stop()
        {
        }

        public const string LoginPageUrlToken         = "{Account.Login.PageUrl}";
        public const string LogoutPageUrlToken        = "{Account.Logout.PageUrl}";

        public const string EditPasswordPageUrlToken = "{Account.EditPassword.PageUrl}";
        public const string UsersPageUrlToken         = "{Account.Users.PageUrl}";
        public const string UsersEditPageUrlToken     = "{Account.Users.Edit.PageUrl}";
        
        public const string RolesPageUrlToken         = "{Account.Roles.PageUrl}";
        
        // See:
        // Framework\N2\Edit\Api\InterfaceBuilder.cs
        // Mvc\MvcTemplates\N2\ManagementItem.cs
        // MvcTemplates\N2\Top.master
        // Dinamico\Themes\Default\Views\Shared\LayoutPartials\LogOn.cshtml

        // TODO: login dialog as part (external providers live in full pages)
        // Dinamico\Themes\Default\Views\ContentParts\LoginForm.cshtml  (ContentPart)

    }
}
