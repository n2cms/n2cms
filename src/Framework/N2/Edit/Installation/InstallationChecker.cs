using System;
using System.Diagnostics;
using N2.Configuration;
using N2.Engine;
using N2.Plugin;
using N2.Web;

namespace N2.Edit.Installation
{
	[Service]
	public class InstallationChecker : IAutoStart
	{
		IWebContext webContext;
		EventBroker broker;
		InstallationManager installer;
		protected bool checkInstallation;
		protected string welcomeUrl;
		protected string managementUrl;

		public InstallationChecker(IWebContext webContext, EventBroker broker, ConfigurationManagerWrapper configuration, InstallationManager installer)
		{
			if (configuration.Sections.Management.Installer.CheckInstallationStatus)
			{
				welcomeUrl = configuration.Sections.Management.Installer.WelcomeUrl;
				managementUrl = configuration.Sections.Management.ManagementInterfaceUrl;
				this.installer = installer;
				this.webContext = webContext;
				this.broker = broker;
				this.broker.BeginRequest += BeginRequest;
			}
		}

		protected void BeginRequest(object sender, EventArgs args)
		{
			CheckInstallation();
		}

		private void CheckInstallation()
		{
			string currentUrl = webContext.ToAppRelative(webContext.Url.LocalUrl);
			bool isEditing = currentUrl.StartsWith(N2.Web.Url.ToRelative(managementUrl), StringComparison.InvariantCultureIgnoreCase);
			if (isEditing)
				return;

			DatabaseStatus status = installer.GetStatus();
			Url redirectUrl = Url.ResolveTokens(welcomeUrl);

			if (status.NeedsUpgrade)
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
				return;
			}
			Trace.WriteLine("Redirecting to '" + redirectUrl + "' to handle status: " + status.ToStatusString());
			webContext.Response.Redirect(redirectUrl);
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
