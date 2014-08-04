using System;
using System.Collections.Generic;
using System.Linq;
using N2.Engine;
using N2.Plugin;

namespace N2.Security
{
    /// <summary>
    /// User account management resources
    /// </summary>
    /// <remarks>
    /// Pages that forms User management subsystem are defined as well-known tokens.
    /// Token values are set at start of application. 
    /// The implementation may be redefined by IoC injection.
    /// <see cref="MembershipAccountResources"/> is default classic membership implementation.
    /// 
    /// TODO: pluggable installation and migration UI/logics.
    /// </remarks>
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

        /// <summary> Login page Url, optional url parameters: returnUrl </summary>
        public const string LoginPageUrlToken        = "{Account.Login.PageUrl}";
        /// <summary> Logout page Url, optional url parameters: - </summary>
        public const string LogoutPageUrlToken       = "{Account.Logout.PageUrl}";

        /// <summary> Manage user profile page Url, optional url parameters: - </summary>
        public const string ManageUserPageUrlToken   = "{Account.ManageUser.PageUrl}";
        /// <summary> Change user password page Url </summary>
        public const string EditPasswordPageUrlToken = "{Account.EditPassword.PageUrl}";

        public const string UsersPageUrlToken        = "{Account.Users.PageUrl}";
        public const string UsersEditPageUrlToken    = "{Account.Users.Edit.PageUrl}";

        public const string RolesPageUrlToken        = "{Account.Roles.PageUrl}";

        // See also:
        //  N2\Edit\Api\InterfaceBuilder.cs             (Framework)
        //  Mvc\MvcTemplates\N2\ManagementItem.cs       (N2.Management)
        //  MvcTemplates\N2\Top.master                  (N2.Management)
        //  Dinamico\Themes\Default\Views\Shared\LayoutPartials\LogOn.cshtml

        // TODO: login part is supported for classic membership only 
        //  (external provider login dialogs are by-design full page)
        //  Dinamico\Themes\Default\Views\ContentParts\LoginForm.cshtml  (ContentPart)

    }
}