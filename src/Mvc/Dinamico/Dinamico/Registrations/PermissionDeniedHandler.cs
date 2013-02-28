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
			var startPage = parser.StartPage as Models.StartPage;
			if (startPage != null && startPage.LoginPage != null)
			{
				e.Cancel = true;
				context.HttpContext.Response.Redirect(Url.Parse(startPage.LoginPage).AppendQuery("returnUrl", context.Url.LocalUrl));
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