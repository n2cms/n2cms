using System;
using System.Web;
using N2.Engine;
using System.Diagnostics;
using N2.Configuration;
using N2.Installation;
using N2.Web.UI;
using Castle.Core;
using N2.Plugin;

namespace N2.Web
{
	/// <summary>
	/// Handles the request life cycle for N2 by invoking url rewriting, 
	/// authorizing and closing NHibernate session.
	/// </summary>
	public class RequestLifeCycleHandler : IRequestLifeCycleHandler, IStartable, IAutoStart
	{
		readonly IErrorHandler errors;
		readonly IWebContext webContext;
		readonly EventBroker broker;
		readonly InstallationManager installer;
		readonly IRequestDispatcher dispatcher;

		protected bool initialized = false;
		protected bool checkInstallation = false;
		protected string installerUrl = "~/Edit/Install/Begin/Default.aspx";

		/// <summary>Creates a new instance of the RequestLifeCycleHandler class.</summary>
		/// <param name="webContext">The web context wrapper.</param>
		public RequestLifeCycleHandler(IWebContext webContext, EventBroker broker, InstallationManager installer, IRequestDispatcher dispatcher, IErrorHandler errors, EditSection editConfig)
            : this(webContext, broker, installer, dispatcher, errors)
        {
			checkInstallation = editConfig.Installer.CheckInstallationStatus;
            installerUrl = editConfig.Installer.InstallUrl;
        }

		/// <summary>Creates a new instance of the RequestLifeCycleHandler class.</summary>
		/// <param name="webContext">The web context wrapper.</param>
		public RequestLifeCycleHandler(IWebContext webContext, EventBroker broker, InstallationManager installer, IRequestDispatcher dispatcher, IErrorHandler errors)
		{
			this.webContext = webContext;
			this.broker = broker;
			this.errors = errors;
			this.installer = installer;
			this.dispatcher = dispatcher;
		}

		/// <summary>Subscribes to applications events.</summary>
		/// <param name="broker">The application.</param>
		public void Init(EventBroker broker)
		{
            Debug.WriteLine("RequestLifeCycleHandler.Init");

			broker.BeginRequest += Application_BeginRequest;
			broker.AuthorizeRequest += Application_AuthorizeRequest;
			broker.AcquireRequestState += Application_AcquireRequestState;
            broker.Error += Application_Error;
			broker.EndRequest += Application_EndRequest;
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

		#region IStartable Members

		public void Start()
		{
			broker.BeginRequest += Application_BeginRequest;
			broker.AuthorizeRequest += Application_AuthorizeRequest;
			broker.AcquireRequestState += Application_AcquireRequestState;
			broker.Error += Application_Error;
			broker.EndRequest += Application_EndRequest;
		}

		public void Stop()
		{
			broker.BeginRequest -= Application_BeginRequest;
			broker.AuthorizeRequest -= Application_AuthorizeRequest;
			broker.AcquireRequestState -= Application_AcquireRequestState;
			broker.Error -= Application_Error;
			broker.EndRequest -= Application_EndRequest;
		}

		#endregion
	}
}