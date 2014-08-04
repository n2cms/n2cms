using System;
using Castle.Core;
using N2.Plugin;
using N2.Security;
using N2.Web;
using N2.Engine;
using N2;

namespace Dinamico.Registrations
{
    [Service]
    public class PermissionDeniedHandler : IAutoStart
    {
        ISecurityEnforcer securityEnforcer;
        IUrlParser parser;
        IWebContext context;

        public PermissionDeniedHandler(ISecurityEnforcer securityEnforcer, IUrlParser parser, IWebContext context)
        {
            this.securityEnforcer = securityEnforcer;
            this.parser = parser;
            this.context = context;
        }

        void securityEnforcer_AuthorizationFailed(object sender, CancellableItemEventArgs e)
        {
            string returnUrl = context.Url.LocalUrl;
            string loginUrl = null;

            // Custom login page:
            var startPage = parser.StartPage as Models.StartPage;
            if (startPage != null && !string.IsNullOrWhiteSpace(startPage.LoginPage))
            {
                loginUrl = startPage.LoginPage;
            }

            // Default login page:
            if (loginUrl == null)
            {
                string loginPageToken = "{Account.Login.PageUrl}";
                string loginPageUrl = loginPageToken.ResolveUrlTokens();
                if (loginPageUrl != loginPageToken)
                {
                    loginUrl = loginPageUrl;
                }
            }

            if (loginUrl != null)
            {
                e.Cancel = true;
                context.HttpContext.Response.Redirect(Url.Parse(loginUrl).AppendQuery("returnUrl", returnUrl));
            }

        }

        #region IStartable Members

        public void Start()
        {
            securityEnforcer.AuthorizationFailed += new EventHandler<CancellableItemEventArgs>(securityEnforcer_AuthorizationFailed);
        }

        public void Stop()
        {
            securityEnforcer.AuthorizationFailed -= new EventHandler<CancellableItemEventArgs>(securityEnforcer_AuthorizationFailed);
        }

        #endregion
    }
}
