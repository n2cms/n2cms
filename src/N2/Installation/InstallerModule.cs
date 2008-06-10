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

namespace N2.Installation
{
    /// <summary>
    /// Performs a database check to detect wether the site has been 
    /// installed. This may cause a redirect for installed sites if there is a
    /// problem connecting to the database. Therefore it's advisable to remove
    /// this http module once the site has been installed.
    /// </summary>
	public class InstallerModule : IHttpModule
	{
        private InstallerSection config;
        private HttpApplication context;
        private IEngine engine;
        private bool alreadyChecked = false;

		public void Init(HttpApplication application)
		{
            config = (InstallerSection)WebConfigurationManager.GetSection("n2/installer");
            if (config != null && config.CheckInstallationStatus)
            {
                this.context = application;
                engine = N2.Context.Initialize(false);
                application.BeginRequest += context_BeginRequest;
            }
		}

		void context_BeginRequest(object sender, EventArgs e)
		{
			if (alreadyChecked || IsEditing())
				return;
			else
				alreadyChecked = true;

			InstallationManager im = new InstallationManager(engine);
			DatabaseStatus status = im.GetStatus();
			if (!status.IsInstalled)
			{
				context.Response.Redirect(config.InstallUrl);
			}
		}

		private bool IsEditing()
		{
			IWebContext web = engine.Resolve<IWebContext>();
			return web.ToAppRelative(web.AbsolutePath).StartsWith("~/Edit", StringComparison.InvariantCultureIgnoreCase);
		}

		public void Dispose()
		{
		}
	}
}
