using System;
using System.Web;
using N2.Persistence.NH;
using N2.Security;
using System.Diagnostics;
using N2.Configuration;

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
        private bool enableRewrite = true;

		/// <summary>Creates a new instance of the RequestLifeCycleHandler class.</summary>
		/// <param name="rewriter">The class that performs url rewriting.</param>
		/// <param name="security">The class that can authorize a request.</param>
		/// <param name="sessionProvider">The class that provides NHibernate sessions.</param>
		/// <param name="webContext">The web context wrapper.</param>
        public RequestLifeCycleHandler(IUrlRewriter rewriter, ISecurityEnforcer security, IWebContext webContext, IErrorHandler errorHandler, HostSection config)
            : this(rewriter, security, webContext, errorHandler)
        {
            enableRewrite = config.Web.RewriteEnabled;
        }

		/// <summary>Creates a new instance of the RequestLifeCycleHandler class.</summary>
		/// <param name="rewriter">The class that performs url rewriting.</param>
		/// <param name="security">The class that can authorize a request.</param>
		/// <param name="sessionProvider">The class that provides NHibernate sessions.</param>
		/// <param name="webContext">The web context wrapper.</param>
        public RequestLifeCycleHandler(IUrlRewriter rewriter, ISecurityEnforcer security, IWebContext webContext, IErrorHandler errorHandler)
		{
			this.rewriter = rewriter;
			this.security = security;
			this.webContext = webContext;
            this.errorHandler = errorHandler;
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
            if (Url.ServerUrl == null && webContext.IsWeb)
            {
                Url.ServerUrl = webContext.HostUrl;
            }

			rewriter.UpdateCurrentPage();
            if(enableRewrite)
                rewriter.RewriteRequest();
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