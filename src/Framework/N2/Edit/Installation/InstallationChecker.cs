using System;
using System.Diagnostics;
using N2.Configuration;
using N2.Engine;
using N2.Plugin;
using N2.Web;
using System.Web.Configuration;
using System.Configuration;

namespace N2.Edit.Installation
{
    [Service]
    public class InstallationChecker : IAutoStart
    {
        private readonly Engine.Logger<InstallationChecker> logger;
        IWebContext webContext;
        EventBroker broker;
        protected bool checkInstallation;
        protected string welcomeUrl;
        protected string managementUrl;
        InstallationManager installer;

        public DatabaseStatus Status
        {
			get { return webContext.RequestItems["InstallationChecker.Status"] as DatabaseStatus ?? (Status = installer.GetStatus()); }
			set { webContext.RequestItems["InstallationChecker.Status"] = value; }
        }

        public InstallationChecker(IWebContext webContext, EventBroker broker, ConfigurationManagerWrapper configuration, InstallationManager installer)
        {
            this.installer = installer;
            if (configuration.Sections.Management.Installer.CheckInstallationStatus)
            {
                welcomeUrl = configuration.Sections.Management.Installer.WelcomeUrl;
                managementUrl = configuration.Sections.Management.Paths.ManagementInterfaceUrl;
                this.webContext = webContext;
                this.broker = broker;
                this.Status = installer.GetStatus();

                installer.UpdateStatus(Status.Level);

                if(Status.Level != SystemStatusLevel.UpAndRunning)
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
                logger.Warn(ex);
            }

            if (Status == null)
            {
                Status = installer.GetStatus();
            }

            Url redirectUrl = Url.ResolveTokens(welcomeUrl);
            if (Status == null)
            {
                Engine.Logger.Warn("Null status");
                installer.UpdateStatus(SystemStatusLevel.Unknown);
                return;
            }
            else if (Status.NeedsUpgrade)
            {
                redirectUrl = redirectUrl.AppendQuery("action", "upgrade");
            }
            else if (!Status.IsInstalled)
            {
                redirectUrl = redirectUrl.AppendQuery("action", "install");
            }
            else if (Status.NeedsRebase)
            {
                redirectUrl = redirectUrl.AppendQuery("action", "rebase");
            }
            else
            {
                this.broker.BeginRequest -= BeginRequest;
                installer.UpdateStatus(Status.Level);
                this.Status = null;
                return;
            }

            installer.UpdateStatus(Status.Level);

            bool isEditing = currentUrl.StartsWith(N2.Web.Url.ToRelative(managementUrl), StringComparison.InvariantCultureIgnoreCase);
            if (isEditing)
                return;

            logger.Debug("Redirecting to '" + redirectUrl + "' to handle status: " + Status.ToStatusString());
            
            this.Status = null;
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
