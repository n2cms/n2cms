using System;
using Dinamico.Models;
using N2;
using N2.Engine;
using N2.Plugin;
using N2.Security;
using N2.Web;

namespace Dinamico.Registrations
{
	[Service]
	public class PermissionDeniedHandler : IAutoStart
	{
		private readonly IWebContext context;
		private readonly IUrlParser parser;
		private readonly ISecurityEnforcer securityEnforcer;

		public PermissionDeniedHandler(ISecurityEnforcer securityEnforcer, IUrlParser parser, IWebContext context)
		{
			this.securityEnforcer = securityEnforcer;
			this.parser = parser;
			this.context = context;
		}

		private void securityEnforcer_AuthorizationFailed(object sender, CancellableItemEventArgs e)
		{
			string returnUrl = context.Url.LocalUrl;
			string loginUrl = null;

			// Custom login page:
			var startPage = parser.StartPage as StartPage;
			if ((startPage != null) && !string.IsNullOrWhiteSpace(startPage.LoginPage))
				loginUrl = startPage.LoginPage;

			// Default login page:
			if (loginUrl == null)
			{
				var loginPageToken = "{Account.Login.PageUrl}";
				var loginPageUrl = loginPageToken.ResolveUrlTokens();
				if (loginPageUrl != loginPageToken)
					loginUrl = loginPageUrl;
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
			securityEnforcer.AuthorizationFailed += securityEnforcer_AuthorizationFailed;
		}

		public void Stop()
		{
			securityEnforcer.AuthorizationFailed -= securityEnforcer_AuthorizationFailed;
		}

		#endregion
	}
}