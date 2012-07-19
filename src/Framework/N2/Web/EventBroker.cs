using System;
using System.Diagnostics;
using System.IO;
using System.Web;
using N2.Engine;
using N2.Plugin.Scheduling;

namespace N2.Web
{
	/// <summary>
	/// A broker for events from the http application. The purpose of this 
	/// class is to reduce the risk of temporary errors during initialization
	/// causing the site to be crippled.
	/// </summary>
	public class EventBroker
	{
		private readonly Engine.Logger<EventBroker> logger;

		static EventBroker()
		{
			Instance = new EventBroker();
		}

		/// <summary>Accesses the event broker singleton instance.</summary>
		public static EventBroker Instance
		{
			get { return Singleton<EventBroker>.Instance; }
			protected set { Singleton<EventBroker>.Instance = value; }
		}

		/// <summary>Attaches to events from the application instance.</summary>
		public virtual void Attach(HttpApplication application)
		{
			logger.InfoFormat("Attaching to {0} ({1})", application, application.GetHashCode());

			application.BeginRequest += Application_BeginRequest;
			application.AuthorizeRequest += Application_AuthorizeRequest;

			application.PostResolveRequestCache += Application_PostResolveRequestCache;
			application.PostMapRequestHandler += Application_PostMapRequestHandler;

			application.AcquireRequestState += Application_AcquireRequestState;
            application.PreRequestHandlerExecute += Application_PreRequestHandlerExecute;
			application.Error += Application_Error;
			application.EndRequest += Application_EndRequest;

			application.Disposed += Application_Disposed;
		}

		public EventHandler<EventArgs> BeginRequest;
		public EventHandler<EventArgs> AuthorizeRequest;
		public EventHandler<EventArgs> PostResolveRequestCache;
		public EventHandler<EventArgs> PostResolveAnyRequestCache;
		public EventHandler<EventArgs> AcquireRequestState;
		public EventHandler<EventArgs> PostMapRequestHandler;
        public EventHandler<EventArgs> PreRequestHandlerExecute;
		public EventHandler<EventArgs> Error;
		public EventHandler<EventArgs> EndRequest;

		protected void Application_BeginRequest(object sender, EventArgs e)
		{
			if (BeginRequest != null && !IsStaticResource(sender))
			{
				logger.DebugFormat("BeginRequest ({0})", sender.GetHashCode());
				BeginRequest(sender, e);
			}
		}

		protected void Application_AuthorizeRequest(object sender, EventArgs e)
		{
			if (AuthorizeRequest != null && !IsStaticResource(sender))
			{
				logger.DebugFormat("AuthorizeRequest ({0})", sender.GetHashCode());
				AuthorizeRequest(sender, e);
			}
		}

		private void Application_PostResolveRequestCache(object sender, EventArgs e)
		{
			if (PostResolveRequestCache != null && !IsStaticResource(sender))
			{
				logger.DebugFormat("PostResolveRequestCache ({0})", sender.GetHashCode());
				PostResolveRequestCache(sender, e);
			}
			if (PostResolveAnyRequestCache != null)
			{
				logger.DebugFormat("PostResolveAnyRequestCache ({0})", sender.GetHashCode());
				PostResolveAnyRequestCache(sender, e);
			}
		}

		private void Application_PostMapRequestHandler(object sender, EventArgs e)
		{
			if (PostMapRequestHandler != null && !IsStaticResource(sender))
			{
				logger.DebugFormat("PostMapRequestHandler ({0})", sender.GetHashCode());
				PostMapRequestHandler(sender, e);
			}
		}

		protected void Application_AcquireRequestState(object sender, EventArgs e)
		{
			if (AcquireRequestState != null && !IsStaticResource(sender))
			{
				logger.DebugFormat("AcquireRequestState ({0})", sender.GetHashCode());
				AcquireRequestState(sender, e);
			}
		}

        protected void Application_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            if (PreRequestHandlerExecute != null && !IsStaticResource(sender))
            {
				logger.DebugFormat("PreRequestHandlerExecute ({0})", sender.GetHashCode());
				PreRequestHandlerExecute(sender, e);
            }
        }

		protected void Application_Error(object sender, EventArgs e)
		{
			if (Error != null && !IsStaticResource(sender))
			{
				logger.DebugFormat("Error ({0})", sender.GetHashCode());
				Error(sender, e);
			}
		}

		protected void Application_EndRequest(object sender, EventArgs e)
		{
			if (EndRequest != null && !IsStaticResource(sender))
			{
				logger.DebugFormat("EndRequest ({0})", sender.GetHashCode());
				EndRequest(sender, e);
			}
		}

		/// <summary>Detaches events from the application instance.</summary>
		void Application_Disposed(object sender, EventArgs e)
		{
			logger.DebugFormat("Disposing ({0})", sender.GetHashCode());
		}

		/// <summary>Returns true if the requested resource is one of the typical resources that needn't be processed by the cms engine.</summary>
		/// <param name="sender">The event sender, probably a http application.</param>
		/// <returns>True if the request targets a static resource file.</returns>
		/// <remarks>
		/// These are the file extensions considered to be static resources:
		/// .css
		///	.gif
		/// .png 
		/// .jpg
		/// .jpeg
		/// .js
		/// .axd
		/// </remarks>
		protected static bool IsStaticResource(object sender)
		{
			HttpApplication application = sender as HttpApplication;
			if(application != null)
			{
				string path = application.Request.Path;

				string extension = VirtualPathUtility.GetExtension(path);
				if(extension == null) return false;
				switch (extension.ToLower())
				{
					case ".gif":
					case ".png":
					case ".jpg":
					case ".jpeg":
					case ".swf":
					case ".js":
					case ".css":
					case ".axd":
						return File.Exists(application.Request.PhysicalPath);
				}
			}
			return false;
		}
	}
}