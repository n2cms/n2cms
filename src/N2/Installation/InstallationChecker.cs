using System;
using System.Collections.Generic;
using System.Text;
using Castle.Core;
using N2.Plugin;
using N2.Configuration;
using N2.Web;

namespace N2.Installation
{
    public class InstallationChecker : IStartable, IAutoStart
    {
        InstallationManager manager;
        IWebContext context;
        EditSection config;

        public InstallationChecker(InstallationManager manager, IWebContext context, EditSection config)
        {
            this.manager = manager;
            this.context = context;
            this.config = config;
        }
        #region IStartable Members

        public void Start()
        {
            if (config.Installer.CheckInstallationStatus && !IsEditing())
            {
                DatabaseStatus status = manager.GetStatus();
                if (!status.IsInstalled)
                {
                    context.Response.Redirect(config.Installer.InstallUrl);
                }
            }
        }

        private bool IsEditing()
        {
            return context.ToAppRelative(context.AbsolutePath).StartsWith("~/edit", StringComparison.InvariantCultureIgnoreCase);
        }

        public void Stop()
        {

        }

        #endregion
    }
}
