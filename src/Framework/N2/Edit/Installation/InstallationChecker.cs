using System;
using System.Diagnostics;
using N2.Configuration;
using N2.Engine;
using N2.Plugin;
using N2.Web;
using System.Web.Configuration;
using System.Configuration;
using log4net;

namespace N2.Edit.Installation
{
	[Service]
	public class InstallationChecker : IAutoStart
	{
		private readonly ILog logger = LogManager.GetLogger(typeof (InstallationChecker));
		IWebContext webContext;
		EventBroker broker;
		protected bool checkInstallation;
		protected string welcomeUrl;
		protected string managementUrl;
		DatabaseStatus status;
		InstallationManager installer;

		public InstallationChecker(IWebContext webContext, EventBroker broker, ConfigurationManagerWrapper configuration, InstallationManager installer)
		{
			this.installer = installer;
			if (configuration.Sections.Management.Installer.CheckInstallationStatus)
			{
				welcomeUrl = configuration.Sections.Management.Installer.WelcomeUrl;
				managementUrl = configuration.Sections.Management.Paths.ManagementInterfaceUrl;
				this.webContext = webContext;
				this.broker = broker;
				this.status = installer.GetStatus();

				installer.UpdateStatus(status.Level);

				if(status.Level != SystemStatusLevel.UpAndRunning)
					this.broker.BeginRequest += BeginRequest;
			}
			else
			{
				installer.UpdateStatus(SystemStatusLevel.Unconfirmed);
			}
		}

		protected void BeginRequest(object sender, EventArgs args)
		{
			CheckInstallation();
		}

		private void CheckInstallation()
		{
			string currentUrl = Url.ToRelative(webContext.Url.LocalUrl);
			
			try 
			{
				AuthenticationSection authentication = ConfigurationManager.GetSection("system.web/authentication") as AuthenticationSection;
				if (currentUrl.Trim('~', '/').StartsWith(Url.ToAbsolute(authentication.Forms.LoginUrl.Trim('~', '/')), StringComparison.InvariantCultureIgnoreCase))
					// don't redirect from login page
					return;
			}
			catch (Exception ex)
			{
				Trace.TraceWarning(ex.ToString());
			}

			if (status == null)
			{
				status = installer.GetStatus();
			}

			Url redirectUrl = Url.ResolveTokens(welcomeUrl);
			if (status == null)
			{
				Trace.TraceWarning("Null status");
				installer.UpdateStatus(SystemStatusLevel.Unknown);
				return;
			}
			else if (status.NeedsUpgrade)
			{
				redirectUrl = redirectUrl.AppendQuery("action", "upgrade");
			}
			else if (!status.IsInstalled)
			{
				redirectUrl = redirectUrl.AppendQuery("action", "install");
			}
			else if (status.NeedsRebase)
			{
				redirectUrl = redirectUrl.AppendQuery("action", "rebase");
			}
			else
			{
				this.broker.BeginRequest -= BeginRequest;
				installer.UpdateStatus(status.Level);
				this.status = null;
				return;
			}

			installer.UpdateStatus(status.Level);

			bool isEditing = currentUrl.StartsWith(N2.Web.Url.ToRelative(managementUrl), StringComparison.InvariantCultureIgnoreCase);
			if (isEditing)
				return;

			logger.Debug("Redirecting to '" + redirectUrl + "' to handle status: " + status.ToStatusString());
			
			this.status = null;
			webContext.HttpContext.Response.Redirect(redirectUrl);
		}

		#region IAutoStart Members

		public void Start()
		{
		}

		public void Stop()
		{
		}

		#endregion
	}
}
