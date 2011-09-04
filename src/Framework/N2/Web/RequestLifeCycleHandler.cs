using System;
using System.Web;
using N2.Configuration;
using N2.Engine;
using N2.Plugin;
using N2.Web.UI;

namespace N2.Web
{
	/// <summary>
	/// Handles the request life cycle for N2 by invoking url rewriting, 
	/// authorizing and closing NHibernate session.
	/// </summary>
	[Service]
	public class RequestLifeCycleHandler : IAutoStart
	{
		private readonly IContentAdapterProvider adapters;
		private readonly EventBroker broker;
		private readonly RequestPathProvider dispatcher;
		private readonly IErrorNotifier errors;
		private readonly IWebContext webContext;

		protected bool initialized;
		protected RewriteMethod rewriteMethod = RewriteMethod.SurroundMapRequestHandler;
		protected string managementUrl;

		/// <summary>Creates a new instance of the RequestLifeCycleHandler class.</summary>
		/// <param name="webContext">The web context wrapper.</param>
		/// <param name="broker"></param>
		/// <param name="dispatcher"></param>
		/// <param name="adapters"></param>
		/// <param name="errors"></param>
		/// <param name="configuration"></param>
		public RequestLifeCycleHandler(IWebContext webContext, EventBroker broker, RequestPathProvider dispatcher, IContentAdapterProvider adapters, IErrorNotifier errors,
									   ConfigurationManagerWrapper configuration)
		{
			rewriteMethod = configuration.Sections.Web.Web.Rewrite;
			managementUrl = configuration.Sections.Management.ManagementInterfaceUrl;
			this.webContext = webContext;
			this.broker = broker;
			this.adapters = adapters;
			this.errors = errors;
			this.dispatcher = dispatcher;
		}

		#region class RewriteMemory

		public class RewriteMemory
		{
			public string OriginalPath { get; set; }
		}

		#endregion

		protected virtual void Application_BeginRequest(object sender, EventArgs e)
		{
			if (!initialized)
			{
				// we need to have reached begin request before we can do certain 
				// things in IIS7. concurrency isn't crucial here.
				initialized = true;
				if (webContext.IsWeb)
				{
					string dummy = Url.ServerUrl; // wayne: DOT NOT REMOVE, initialize the server url
					Url.SetToken(Url.ManagementUrlToken, Url.ToAbsolute(managementUrl).TrimEnd('/'));
					Url.SetToken("{IconsUrl}", Url.ResolveTokens(Url.ManagementUrlToken + "/Resources/icons"));
				}
			}
			PathData data = dispatcher.GetCurrentPath();
			webContext.CurrentPath = data;

			if (rewriteMethod == RewriteMethod.BeginRequest && data != null && !data.IsEmpty())
			{
				var adapter = adapters.ResolveAdapter<RequestAdapter>(data.CurrentPage);
				adapter.RewriteRequest(data, rewriteMethod);
			}
		}

		protected virtual void Application_AuthorizeRequest(object sender, EventArgs e)
		{
			if (webContext.CurrentPath != null && !webContext.CurrentPath.IsEmpty())
			{
				var adapter = adapters.ResolveAdapter<RequestAdapter>(webContext.CurrentPage);
				adapter.AuthorizeRequest(webContext.CurrentPath, webContext.User);
			}
		}

		protected virtual void Application_PostResolveRequestCache(object sender, EventArgs e)
		{
			if (rewriteMethod == RewriteMethod.SurroundMapRequestHandler)
			{
				PathData data = webContext.CurrentPath;
				if (data != null && !data.IsEmpty())
				{
					var adapter = adapters.ResolveAdapter<RequestAdapter>(data.CurrentPage);

					webContext.RequestItems[this] = new RewriteMemory { OriginalPath = webContext.Url.LocalUrl };
					adapter.RewriteRequest(data, rewriteMethod);
				}
			}
		}

		protected virtual void Application_PostMapRequestHandler(object sender, EventArgs e)
		{
			if (rewriteMethod == RewriteMethod.SurroundMapRequestHandler)
			{
				var info = webContext.RequestItems[this] as RewriteMemory;
				if (info != null)
				{
					Url path = info.OriginalPath;
					webContext.HttpContext.RewritePath(path.Path, "", path.Query ?? "");
				}
			}
		}

		/// <summary>Infuses the http handler (usually an aspx page) with the content page associated with the url if it implements the <see cref="IContentTemplate"/> interface.</summary>
		protected virtual void Application_AcquireRequestState(object sender, EventArgs e)
		{
			if (webContext.CurrentPath == null || webContext.CurrentPath.IsEmpty()) return;

			var adapter = adapters.ResolveAdapter<RequestAdapter>(webContext.CurrentPage);
			adapter.InjectCurrentPage(webContext.CurrentPath, webContext.HttpContext.Handler);
		}

		protected virtual void Application_Error(object sender, EventArgs e)
		{
			var application = sender as HttpApplication;
			if (application != null)
			{
				Exception ex = application.Server.GetLastError();
				if (ex != null && new HttpException(null, ex).GetHttpCode() != 404)
					// we should not notify 404 errors, otherwise, the maxErrorReportsPerHour limit will soon be exceeds.
				{
					errors.Notify(ex);
				}
			}
		}

		protected virtual void Application_EndRequest(object sender, EventArgs e)
		{
			webContext.Close();
		}

		#region IAutoStart Members

		public void Start()
		{
			broker.BeginRequest += Application_BeginRequest;
			broker.PostResolveRequestCache += Application_PostResolveRequestCache;
			broker.PostMapRequestHandler += Application_PostMapRequestHandler;
			broker.AuthorizeRequest += Application_AuthorizeRequest;
			broker.AcquireRequestState += Application_AcquireRequestState;
			broker.Error += Application_Error;
			broker.EndRequest += Application_EndRequest;
		}

		public void Stop()
		{
			broker.BeginRequest -= Application_BeginRequest;
			broker.PostResolveRequestCache -= Application_PostResolveRequestCache;
			broker.PostMapRequestHandler -= Application_PostMapRequestHandler;
			broker.AuthorizeRequest -= Application_AuthorizeRequest;
			broker.AcquireRequestState -= Application_AcquireRequestState;
			broker.Error -= Application_Error;
			broker.EndRequest -= Application_EndRequest;
		}

		#endregion
	}
}