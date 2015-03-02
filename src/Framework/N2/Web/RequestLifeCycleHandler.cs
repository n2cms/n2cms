using System;
using System.Web;
using N2.Configuration;
using N2.Engine;
using N2.Plugin;
using N2.Web.UI;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// uncomment this line to turn on Safe URL Handling (has some bugs):
// #define SAFE_URL_HANDLING

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
        private string selectedQueryKey;

#if SAFE_URL_HANDLING
		#region Utlity Functions - Sorry Wasn't sure where these should go! --jamestharpe

		static void RedirectPermanent(HttpResponse response, string destination)
		{
			response.Clear();
			response.Status = "301 Moved Permanently";
			response.AddHeader("Location", destination);
			response.Flush();
			response.End();
		}

		/// <summary>
		/// Get's the <see cref="ISitesSource"/> provider for the given <see cref="ContentItem"/>.
		/// </summary>
		/// <param name="contentItem"></param>
		/// <returns></returns>
		static ISitesSource GetSitesSource(ContentItem contentItem)
		{
			if (contentItem == null)
				return Find.StartPage as ISitesSource;

			N2.ContentItem currentItem = contentItem;
			do
			{
				ISitesSource result = currentItem as ISitesSource;
				if (result != null)
					return result;

				currentItem = currentItem.Parent;
			} while (currentItem.Parent != null);

			return Find.StartPage as ISitesSource;
		}

		static string GetPreferredUrl(N2.ContentItem contentItem, Uri requestBaseUri, bool cacheResult = true)
		{
			//
			// Pre-conditions

			if (contentItem == null)
				return string.Empty;
			if (!contentItem.IsPage)
				return contentItem.Url;

			//
			// Determine base-url/authority from sites source and requested URL.

			HttpContext httpContext = HttpContext.Current;
			HttpRequest request = httpContext.Request;
			N2.Web.ISitesSource sitesSource = GetSitesSource(contentItem);
			IEnumerable<N2.Web.Site> sites = sitesSource.GetSites();
			N2.Web.Site site = sites // Try to match domain, otherwise default to first-available authority
				.Where(s =>
					s.Authority.Equals(request.Url.Authority, StringComparison.OrdinalIgnoreCase))
				.FirstOrDefault()
				?? sites.FirstOrDefault();

			Uri siteBaseUrl = (site != null) // "Safe" URLs are fully qualified
				? new Uri(string.Format("{0}{1}{2}", request.Url.Scheme, Uri.SchemeDelimiter, site.Authority))
				: requestBaseUri;

			//
			// Check cache

			System.Web.Caching.Cache cache = httpContext.Cache;
			cacheResult = cacheResult && cache != null;
			string cacheKey = null;

			if (cacheResult)
			{
				cacheKey = string.Format(
					"RequestLifeCycleHandler.PreferredUrl_{0}{1}{2}{3}",
					contentItem.ID,
					contentItem.Updated.Ticks,
					contentItem.State, // forces re-calc when a page is published
					siteBaseUrl);
				string cachedResult = cache[cacheKey] as string;
				if (!string.IsNullOrEmpty(cachedResult))
					return cachedResult;
			}

			//
			// Not in cache, calc result

			var parent = contentItem.Parent ?? contentItem.VersionOf.Parent;
			string result;
			if ((parent == null) || (parent == Find.RootItem))
				result = siteBaseUrl.ToString(); // contentItem.Url.ToUri(siteBaseUrl).ToString();
			else
			{
				result = new Uri(
					siteBaseUrl,
					contentItem.Url).ToString();
			}

			//
			// Cache result

			if (cacheResult)
				cache[cacheKey] = result;

			return result;
		}
		#endregion
#endif

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
			managementUrl = configuration.Sections.Management.Paths.ManagementInterfaceUrl;
            selectedQueryKey = configuration.Sections.Management.Paths.SelectedQueryKey;
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
                    Url.SetToken(Url.SelectedQueryKeyToken, selectedQueryKey);
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
			var path = webContext.CurrentPath;
			if (path == null || path.IsEmpty())
				return;

			if (path.IsPubliclyAvailable)
				return;

			var adapter = adapters.ResolveAdapter<RequestAdapter>(path.CurrentPage);
			adapter.AuthorizeRequest(path, webContext.User);
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

		//TODO: Add ForceConsistentUrls property? Right now there is only this #if to turn it off by default.
#if SAFE_URL_HANDLING
		/// <summary>Infuses the http handler (usually an aspx page) with the content page associated with the url if it implements the <see cref="IContentTemplate"/> interface.</summary>
		protected virtual void §(object sender, EventArgs e)
		{

			if (webContext.CurrentPath == null || webContext.CurrentPath.IsEmpty()) return;

			HttpContext httpContext = ((HttpApplication)sender).Context; // jamestharpe: webContext.Request causes Obsolete warning.
			Uri requestBaseUrl = new Uri(string.Format("{0}{1}{2}", httpContext.Request.Url.Scheme, Uri.SchemeDelimiter, httpContext.Request.Url.Authority));
			string
				rawUrl = new Uri(requestBaseUrl, httpContext.Request.RawUrl).ToString(),
				preferredUrl = GetPreferredUrl(webContext.CurrentPage, requestBaseUrl);

			if (!rawUrl.Equals(preferredUrl, StringComparison.InvariantCulture))
			{
				int queryStringIndex = rawUrl.IndexOf('?');
				if (queryStringIndex < 0) // Not equal to SafeUrl - redirect
					RedirectPermanent(httpContext.Response, preferredUrl);
				else if (queryStringIndex != preferredUrl.Length)
				{
					// There was a query string - this could have caused the difference
					string
						queryString = rawUrl.Substring(queryStringIndex),
						destination = preferredUrl + queryString;
					RedirectPermanent(httpContext.Response, destination);
				}
			}
			else
			{
				var adapter = adapters.ResolveAdapter<RequestAdapter>(webContext.CurrentPage);
				adapter.InjectCurrentPage(webContext.CurrentPath, webContext.HttpContext.Handler);
			}

		}
#endif

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
#if SAFE_URL_HANDLING
			broker.AcquireRequestState += Application_AcquireRequestState;
#endif
			broker.Error += Application_Error;
			broker.EndRequest += Application_EndRequest;
		}

		public void Stop()
		{
			broker.BeginRequest -= Application_BeginRequest;
			broker.PostResolveRequestCache -= Application_PostResolveRequestCache;
			broker.PostMapRequestHandler -= Application_PostMapRequestHandler;
			broker.AuthorizeRequest -= Application_AuthorizeRequest;
#if SAFE_URL_HANDLING
			broker.AcquireRequestState -= Application_AcquireRequestState;
#endif
			broker.Error -= Application_Error;
			broker.EndRequest -= Application_EndRequest;
		}

		#endregion
	}
}