using System;
using System.Web;
using N2.Persistence.NH;
using N2.Security;
using System.Diagnostics;
using N2.Configuration;
using N2.Installation;

namespace N2.Web
{
	/// <summary>
	/// Handles the request life cycle for N2 by invoking url rewriting, 
	/// authorizing and closing NHibernate session.
	/// </summary>
	public class RequestLifeCycleHandler : IRequestLifeCycleHandler
	{
		private readonly IUrlRewriter rewriter;
		private readonly ISecurityEnforcer security;
		private readonly IWebContext webContext;
        private readonly IErrorHandler errorHandler;
        private readonly InstallationManager installer;

        private bool initialized = false;
        private bool checkInstallation = false;
        private string installerUrl = "~/Edit/Install/Begin/Default.aspx";

		/// <summary>Creates a new instance of the RequestLifeCycleHandler class.</summary>
		/// <param name="rewriter">The class that performs url rewriting.</param>
		/// <param name="security">The class that can authorize a request.</param>
		/// <param name="webContext">The web context wrapper.</param>
        public RequestLifeCycleHandler(IUrlRewriter rewriter, ISecurityEnforcer security, IWebContext webContext, IErrorHandler errorHandler, InstallationManager installer, EditSection editConfig, HostSection hostConfig)
            : this(rewriter, security, webContext, errorHandler, installer)
        {
            checkInstallation = editConfig.Installer.CheckInstallationStatus;
            installerUrl = editConfig.Installer.InstallUrl;
        }

		/// <summary>Creates a new instance of the RequestLifeCycleHandler class.</summary>
		/// <param name="rewriter">The class that performs url rewriting.</param>
		/// <param name="security">The class that can authorize a request.</param>
		/// <param name="webContext">The web context wrapper.</param>
        public RequestLifeCycleHandler(IUrlRewriter rewriter, ISecurityEnforcer security, IWebContext webContext, IErrorHandler errorHandler, InstallationManager installer)
		{
			this.rewriter = rewriter;
			this.security = security;
			this.webContext = webContext;
            this.errorHandler = errorHandler;
            this.installer = installer;
		}

		/// <summary>Subscribes to applications events.</summary>
		/// <param name="application">The application.</param>
		public void Init(HttpApplication application)
		{
            Debug.WriteLine("RequestLifeCycleHandler.Init");

			application.BeginRequest += Application_BeginRequest;
			application.AuthorizeRequest += Application_AuthorizeRequest;
			application.AcquireRequestState += Application_AcquireRequestState;
            application.Error += Application_Error;
			application.EndRequest += Application_EndRequest;
		}

		protected virtual void Application_BeginRequest(object sender, EventArgs e)
		{
            if (!initialized)
            {
                // we need to have reached begin request before we can do certain 
                // things in IIS7. concurrency isn't crucial here.
                initialized = true;
                if (webContext.IsWeb)
                {
                    if (Url.ServerUrl == null)
                        Url.ServerUrl = webContext.Url.HostUrl;
                    if (checkInstallation)
                        CheckInstallation();
                }
            }

			rewriter.InitializeRequest();
            rewriter.RewriteRequest();
		}

        private void CheckInstallation()
        {
            bool isEditing = webContext.ToAppRelative(webContext.Url.LocalUrl).StartsWith("~/edit", StringComparison.InvariantCultureIgnoreCase);
            if (!isEditing && !installer.GetStatus().IsInstalled)
            {
                webContext.Response.Redirect(installerUrl);
            }
        }

		protected virtual void Application_AcquireRequestState(object sender, EventArgs e)
		{
			rewriter.InjectContentPage();
		}

		protected virtual void Application_AuthorizeRequest(object sender, EventArgs e)
		{
			security.AuthorizeRequest();
		}

        protected virtual void Application_EndRequest(object sender, EventArgs e)
        {
            webContext.Dispose();
        }

        protected virtual void Application_Error(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            if(application != null)
                errorHandler.Notify(application.Server.GetLastError());
        }
	}
}