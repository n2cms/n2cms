using System;
using System.Web;
using N2.Persistence.NH;
using N2.Security;
using System.Diagnostics;

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
		private readonly ISessionProvider sessionProvider;
		private readonly IWebContext webContext;

		/// <summary>Creates a new instance of the RequestLifeCycleHandler class.</summary>
		/// <param name="rewriter">The class that performs url rewriting.</param>
		/// <param name="security">The class that can authorize a request.</param>
		/// <param name="sessionProvider">The class that provides NHibernate sessions.</param>
		/// <param name="webContext">The web context wrapper.</param>
		public RequestLifeCycleHandler(IUrlRewriter rewriter, ISecurityEnforcer security, ISessionProvider sessionProvider, IWebContext webContext)
		{
			this.rewriter = rewriter;
			this.security = security;
			this.sessionProvider = sessionProvider;
			this.webContext = webContext;
		}

		/// <summary>Subscribes to applications events.</summary>
		/// <param name="application">The application.</param>
		public void Init(HttpApplication application)
		{
			Debug.WriteLine("RequestLifeCycleHandler.Init");

			application.BeginRequest += Application_BeginRequest;
			application.AuthorizeRequest += Application_AuthorizeRequest;
			application.EndRequest += Application_EndRequest;
			application.AcquireRequestState += Application_AcquireRequestState;
		}

		protected virtual void Application_BeginRequest(object sender, EventArgs e)
		{
			Debug.WriteLine("Beginning: " + webContext.RawUrl);

			rewriter.UpdateCurrentPage();
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
			sessionProvider.Dispose();
		}
	}
}