using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Diagnostics;
using N2.Web;
using System.Configuration;
using N2.Configuration;
using System.Web.Configuration;
using N2.Engine;
using N2.Persistence.NH;
using N2.Persistence;
using N2.Definitions;
using N2.Serialization;

namespace N2.Installation
{
    /// <summary>
    /// Performs a database check to detect wether the site has been 
    /// installed. This may cause a redirect for installed sites if there is a
    /// problem connecting to the database. Therefore it's advisable to remove
    /// this http module once the site has been installed.
    /// </summary>
    [Obsolete]
	public class InstallerModule : IHttpModule
	{
        private EditSection config;
        private HttpApplication context;
        private IEngine engine;
        private bool alreadyChecked = false;

		public void Init(HttpApplication application)
		{
            config = (EditSection)WebConfigurationManager.GetSection("n2/edit");
            if (config != null && config.Installer.CheckInstallationStatus)
            {
                context = application;
                application.BeginRequest += context_BeginRequest;

                Context.Initialize(false);
            	engine = Context.Current;
            }
		}

		void context_BeginRequest(object sender, EventArgs e)
		{
			if (alreadyChecked || IsEditing())
				return;
			else
				alreadyChecked = true;

            InstallationManager im = engine.Resolve<InstallationManager>();
			DatabaseStatus status = im.GetStatus();
			if (!status.IsInstalled)
			{
                context.Response.Redirect(config.Installer.InstallUrl);
			}
		}

		private bool IsEditing()
		{
			IWebContext web = engine.Resolve<IWebContext>();
			return web.Url.LocalUrl.ApplicationRelativePath.StartsWith("~/edit", StringComparison.InvariantCultureIgnoreCase);
		}

		public void Dispose()
		{
		}
	}
}
