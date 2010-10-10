using System;
using System.Web;
using N2.Engine;
using System.Diagnostics;
using N2.Configuration;
using N2.Installation;
using N2.Web.UI;
using N2.Plugin;
using N2.Edit.Installation;

namespace N2.Web
{
	/// <summary>
	/// Handles the request life cycle for N2 by invoking url rewriting, 
	/// authorizing and closing NHibernate session.
	/// </summary>
	[Service(typeof(IRequestLifeCycleHandler))]
	public class RequestLifeCycleHandler : IRequestLifeCycleHandler
	{
		readonly IErrorHandler errors;
		readonly IWebContext webContext;
		readonly EventBroker broker;
		readonly InstallationManager installer;
		readonly RequestPathProvider dispatcher;
		readonly IContentAdapterProvider adapters;

		protected bool initialized = false;
		protected bool checkInstallation = false;
		protected RewriteMethod rewriteMethod = RewriteMethod.RewriteRequest;
		protected string installerUrl = "~/N2/Installation/Begin/Default.aspx";

		/// <summary>Creates a new instance of the RequestLifeCycleHandler class.</summary>
		/// <param name="webContext">The web context wrapper.</param>
		/// <param name="broker"></param>
		/// <param name="installer"></param>
		/// <param name="dispatcher"></param>
		/// <param name="errors"></param>
		/// <param name="editConfig"></param>
		/// <param name="hostConfig"></param>
		public RequestLifeCycleHandler(IWebContext webContext, EventBroker broker, InstallationManager installer, RequestPathProvider dispatcher, IContentAdapterProvider adapters, IErrorHandler errors, EditSection editConfig, HostSection hostConfig)
        {
			checkInstallation = editConfig.Installer.CheckInstallationStatus;
            installerUrl = editConfig.Installer.InstallUrl;
			rewriteMethod = hostConfig.Web.Rewrite;
			this.webContext = webContext;
			this.broker = broker;
			this.adapters = adapters;
			this.errors = errors;
			this.installer = installer;
			this.dispatcher = dispatcher;
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
				    var dummy = Url.ServerUrl;  // wayne: DOT NOT REMOVE, initialize the server url
					if (checkInstallation)
						CheckInstallation();
				}
			}

			var data = dispatcher.GetCurrentPath();
			webContext.CurrentPath = data;
			if (data != null && !data.IsEmpty())
			{
				RequestAdapter adapter = adapters.ResolveAdapter<RequestAdapter>(data.CurrentPage.GetContentType());
				adapter.RewriteRequest(data, rewriteMethod);
			}
		}
		
		protected virtual void Application_AuthorizeRequest(object sender, EventArgs e)
		{
			if (webContext.CurrentPath != null && !webContext.CurrentPath.IsEmpty())
			{
				RequestAdapter adapter = adapters.ResolveAdapter<RequestAdapter>(webContext.CurrentPage.GetContentType());
				adapter.AuthorizeRequest(webContext.CurrentPath, webContext.User);
			}
		}

		/// <summary>Infuses the http handler (usually an aspx page) with the content page associated with the url if it implements the <see cref="IContentTemplate"/> interface.</summary>
		protected virtual void Application_AcquireRequestState(object sender, EventArgs e)
		{
			if (webContext.CurrentPath == null || webContext.CurrentPath.IsEmpty()) return;

			RequestAdapter adapter = adapters.ResolveAdapter<RequestAdapter>(webContext.CurrentPage.GetContentType());
			adapter.InjectCurrentPage(webContext.CurrentPath, webContext.Handler);
		}

        protected virtual void Application_Error(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            if(application != null)
            {
            	Exception ex = application.Server.GetLastError();
                if (ex != null && new HttpException(null, ex).GetHttpCode() != 404)  // we should not notify 404 errors, otherwise, the maxErrorReportsPerHour limit will soon be exceeds.
				{
					errors.Notify(ex);
				}
            }
		}

		protected virtual void Application_EndRequest(object sender, EventArgs e)
		{
			webContext.Close();
		}

		private void CheckInstallation()
		{
			bool isEditing = webContext.ToAppRelative(webContext.Url.LocalUrl).StartsWith("~/N2/", StringComparison.InvariantCultureIgnoreCase);
			if (!isEditing && !installer.GetStatus().IsInstalled)
			{
				webContext.Response.Redirect(installerUrl);
			}
		}

		#region IRequestLifeCycleHandler Members

		public void Initialize()
		{
			broker.BeginRequest += Application_BeginRequest;
			broker.AuthorizeRequest += Application_AuthorizeRequest;
			broker.AcquireRequestState += Application_AcquireRequestState;
			broker.Error += Application_Error;
			broker.EndRequest += Application_EndRequest;
		}

		#endregion
	}
}