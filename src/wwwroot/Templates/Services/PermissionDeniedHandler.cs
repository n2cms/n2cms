using System;
using Castle.Core;
using N2.Plugin;
using N2.Security;
using N2.Web;
using N2.Templates.Items;

namespace N2.Templates.Services
{
    public class PermissionDeniedHandler : IStartable, IAutoStart
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

        void securityEnforcer_AuthorizationFailed(object sender, N2.Persistence.CancellableItemEventArgs e)
        {
            AbstractStartPage startPage = parser.StartPage as AbstractStartPage;
            if (startPage != null && startPage.LoginPage != null)
            {
                e.Cancel = true;
                context.Response.Redirect(Url.Parse(startPage.LoginPage.Url).AppendQuery("returnUrl", context.LocalUrl));
            }
        }

        #region IStartable Members

        public void Start()
        {
            securityEnforcer.AuthorizationFailed += new EventHandler<N2.Persistence.CancellableItemEventArgs>(securityEnforcer_AuthorizationFailed);
        }

        public void Stop()
        {
            securityEnforcer.AuthorizationFailed -= new EventHandler<N2.Persistence.CancellableItemEventArgs>(securityEnforcer_AuthorizationFailed);
        }

        #endregion
    }
}
