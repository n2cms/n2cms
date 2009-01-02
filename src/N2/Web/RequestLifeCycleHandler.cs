using System;
using System.Web;
using N2.Persistence.NH;
using N2.Security;
using System.Diagnostics;
using N2.Configuration;
using N2.Installation;
using N2.Web.UI;

namespace N2.Web
{
	/// <summary>
	/// Handles the request life cycle for N2 by invoking url rewriting, 
	/// authorizing and closing NHibernate session.
	/// </summary>
	public class RequestLifeCycleHandler : IRequestLifeCycleHandler
	{
		readonly ISecurityEnforcer security;
		readonly IWebContext webContext;
        readonly IErrorHandler errorHandler;
        readonly InstallationManager installer;
		readonly IRequestDispatcher dispatcher;

		protected bool initialized = false;
		protected bool checkInstallation = false;
		protected string installerUrl = "~/Edit/Install/Begin/Default.aspx";

		/// <summary>Creates a new instance of the RequestLifeCycleHandler class.</summary>
		/// <param name="security">The class that can authorize a request.</param>
		/// <param name="webContext">The web context wrapper.</param>
		public RequestLifeCycleHandler(ISecurityEnforcer security, IWebContext webContext, IErrorHandler errorHandler, InstallationManager installer, IRequestDispatcher dispatcher, EditSection editConfig, HostSection hostConfig)
            : this(security, webContext, errorHandler, installer, dispatcher)
        {
            checkInstallation = editConfig.Installer.CheckInstallationStatus;
            installerUrl = editConfig.Installer.InstallUrl;
        }

		/// <summary>Creates a new instance of the RequestLifeCycleHandler class.</summary>
		/// <param name="security">The class that can authorize a request.</param>
		/// <param name="webContext">The web context wrapper.</param>
        public RequestLifeCycleHandler(ISecurityEnforcer security, IWebContext webContext, IErrorHandler errorHandler, InstallationManager installer, IRequestDispatcher dispatcher)
		{
			this.security = security;
			this.webContext = webContext;
            this.errorHandler = errorHandler;
            this.installer = installer;
			this.dispatcher = dispatcher;
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

			IRequestController controller = dispatcher.ResolveController<IRequestController>();
			if(controller != null)
			{
				webContext.CurrentPath = controller.Path;
				RequestItem<IRequestController>.Instance = controller;
				controller.RewriteRequest(webContext);
			}
		}

        private void CheckInstallation()
        {
            bool isEditing = webContext.ToAppRelative(webContext.Url.LocalUrl).StartsWith("~/edit", StringComparison.InvariantCultureIgnoreCase);
            if (!isEditing && !installer.GetStatus().IsInstalled)
            {
                webContext.Response.Redirect(installerUrl);
            }
        }

		/// <summary>Infuses the http handler (usually an aspx page) with the content page associated with the url if it implements the <see cref="IContentTemplate"/> interface.</summary>
		protected virtual void Application_AcquireRequestState(object sender, EventArgs e)
		{
			RequestItem<IRequestController>.Instance.InjectCurrentPage(webContext.Handler);
		}

		protected virtual void Application_AuthorizeRequest(object sender, EventArgs e)
		{
			IRequestController controller = RequestItem<IRequestController>.Instance;
			if (controller != null)
			{
				controller.AuthorizeRequest(webContext.User, security);
			}
		}

        protected virtual void Application_Error(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            if(application != null)
            {
            	Exception ex = application.Server.GetLastError();
				if(ex != null)
				{
					errorHandler.Notify(ex);
					IRequestController controller = RequestItem<IRequestController>.Instance;
					if(controller != null)
					{
						controller.HandleError(ex);
					}
				}
            }
		}

		protected virtual void Application_EndRequest(object sender, EventArgs e)
		{
			webContext.Close();
		}
	}
}