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
		readonly IErrorHandler errors;
		readonly IWebContext webContext;
        readonly InstallationManager installer;
		readonly IRequestDispatcher dispatcher;

		protected bool initialized = false;
		protected bool checkInstallation = false;
		protected string installerUrl = "~/Edit/Install/Begin/Default.aspx";

		/// <summary>Creates a new instance of the RequestLifeCycleHandler class.</summary>
		/// <param name="webContext">The web context wrapper.</param>
		public RequestLifeCycleHandler(IWebContext webContext, InstallationManager installer, IRequestDispatcher dispatcher, IErrorHandler errors, EditSection editConfig)
            : this(webContext, installer, dispatcher, errors)
        {
			checkInstallation = editConfig.Installer.CheckInstallationStatus;
            installerUrl = editConfig.Installer.InstallUrl;
        }

		/// <summary>Creates a new instance of the RequestLifeCycleHandler class.</summary>
		/// <param name="webContext">The web context wrapper.</param>
		public RequestLifeCycleHandler(IWebContext webContext, InstallationManager installer, IRequestDispatcher dispatcher, IErrorHandler errors)
		{
			this.webContext = webContext;
			this.errors = errors;
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

			RequestAspectController controller = dispatcher.ResolveAspectController<RequestAspectController>();
			if(controller != null)
			{
				webContext.CurrentPath = controller.Path;
				controller.RewriteRequest();
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
			if (webContext.CurrentPath == null || webContext.CurrentPath.IsEmpty()) return;

			RequestAspectController controller = dispatcher.ResolveAspectController<RequestAspectController>();
			controller.InjectCurrentPage(webContext.Handler);
		}

		protected virtual void Application_AuthorizeRequest(object sender, EventArgs e)
		{
			if (webContext.CurrentPath == null || webContext.CurrentPath.IsEmpty()) return;

			RequestAspectController controller = dispatcher.ResolveAspectController<RequestAspectController>();
			controller.AuthorizeRequest(webContext.User);
		}

        protected virtual void Application_Error(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            if(application != null)
            {
            	Exception ex = application.Server.GetLastError();
				if(ex != null)
				{
					errors.Notify(ex);
				}
            }
		}

		protected virtual void Application_EndRequest(object sender, EventArgs e)
		{
			webContext.Close();
		}
	}
}