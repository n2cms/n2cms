using System.Collections;
using System.Security.Principal;
using System.Web;
using System.Diagnostics;
using System.Collections.Specialized;

namespace N2.Web
{
	/// <summary>
	/// A wrapper for web methods and properties available through 
	/// <see cref="System.Web.HttpContext.Current"/>.
	/// </summary>
	public class RequestContext : IWebContext
	{
		public RequestContext()
		{
			Debug.WriteLine("RequestContext");
		}

		public HttpContext CurrentHttpContext
		{
			get 
			{
				if (HttpContext.Current == null)
					throw new N2Exception("Tried to retrieve HttpContext.Current but it's null. This may happen when working outside a request or when doing stuff after the context has been recycled.");
				return HttpContext.Current; 
			}
		}

		/// <summary>Gets wether there is a web context availabe.</summary>
		public bool IsInWebContext
		{
			get { return CurrentHttpContext != null; }
		}

		/// <summary>Gets a dictionary of request scoped items.</summary>
		public virtual IDictionary RequestItems
		{
			get { return IsInWebContext ? CurrentHttpContext.Items : null; }
		}

		/// <summary>The handler associated with this request.</summary>
		public virtual IHttpHandler Handler 
		{
			get { return CurrentHttpContext.Handler; }
		}

		/// <summary>Gets the current host name.</summary>
		public virtual string Host
		{
			get { return Request.Url.Authority; }
		}

		/// <summary>Gets the url relative to the application root.</summary>
		public virtual string RawUrl
		{
			get { return Request.RawUrl; }
		}

		/// <summary>Gets the application root path.</summary>
		public virtual string ApplicationUrl
		{
			get { return Request.ApplicationPath; }
		}

		/// <summary>Gets the current request's query string.</summary>
		public virtual NameValueCollection QueryString
		{
			get { return Request.QueryString; }
		}

		/// <summary>Gets the current request's query string.</summary>
		public virtual string Query
		{
			get { return QueryString.ToString(); }
		}

		/// <summary>The path to the requested resource without query string.</summary>
		public virtual string AbsolutePath
		{
			get
			{
				return PathPart(RawUrl);
			}
		}

		public static string RemoveHash(string url)
		{
			int hashIndex = url.IndexOf('#');
			if (hashIndex >= 0)
				url = url.Substring(0, hashIndex);
			return url;
		}

		/// <summary>Retrieves the path part of an url, e.g. /path/to/page.aspx.</summary>
		public static string PathPart(string url)
		{
			url = RemoveHash(url);

			int queryIndex = url.IndexOf('?');
			if (queryIndex >= 0)
				url = url.Substring(0, queryIndex);

			return url;
		}

		/// <summary>Retrieves the query part of an url, e.g. page=12&value=something.</summary>
		public static string QueryPart(string url)
		{
			url = RemoveHash(url);

			int queryIndex = url.IndexOf('?');
			if (queryIndex >= 0)
				return url.Substring(queryIndex + 1);
			return string.Empty;
		}

		/// <summary>The physical path on disk to the requested resource.</summary>
		public virtual string PhysicalPath
		{
			get { return Request.PhysicalPath; }
		}

		/// <summary>Gets the current user in the web execution context.</summary>
		public virtual IPrincipal User
		{
			get { return CurrentHttpContext.User; }
		}

		/// <summary>A page instance stored in the request context.</summary>
		public ContentItem CurrentPage
		{
			get { return RequestItems["CurrentPage"] as ContentItem; }
			set { RequestItems["CurrentPage"] = value; }
		}

		/// <summary>The current request object.</summary>
		public virtual HttpRequest Request
		{
			get { return CurrentHttpContext.Request; }
		}

		public virtual HttpCookieCollection Cookies
		{
			get { return Request.Cookies; }
		}

		/// <summary>The current request object.</summary>
		public virtual HttpResponse Response
		{
			get { return CurrentHttpContext.Response; }
		}

		/// <summary>Converts a virtual url to an absolute url.</summary>
		/// <param name="virtualPath">The virtual url to make absolute.</param>
		/// <returns>The absolute url.</returns>
		public virtual string ToAbsolute(string virtualPath)
		{
			return Utility.ToAbsolute(virtualPath);
		}

		/// <summary>Converts an absolute url to an app relative url.</summary>
		/// <param name="virtualPath">The absolute url to convert.</param>
		/// <returns>An app relative url.</returns>
		public virtual string ToAppRelative(string virtualPath)
		{
			if (virtualPath != null && virtualPath.StartsWith(Utility.WebRootPath, System.StringComparison.InvariantCultureIgnoreCase))
				return "~/" + virtualPath.Substring(Utility.WebRootPath.Length);
			return virtualPath;
		}

		/// <summary>Maps a virtual path to a physical disk path.</summary>
		/// <param name="path">The path to map. E.g. "~/bin"</param>
		/// <returns>The physical path. E.g. "c:\inetpub\wwwroot\bin"</returns>
		public virtual string MapPath(string path)
		{
			return CurrentHttpContext.Server.MapPath(path);
		}

		/// <summary>Assigns a rewrite path.</summary>
		/// <param name="path">The path to the template that will handle the request.</param>
		public virtual void RewritePath(string path)
		{
			Debug.WriteLine("Rewriting '" + RawUrl + "' to '" + path + "'");
			CurrentHttpContext.RewritePath(path, false);
		}
	}
}
