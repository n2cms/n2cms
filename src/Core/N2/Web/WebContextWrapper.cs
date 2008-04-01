using System.Collections;
using System.Security.Principal;
using System.Web;

namespace N2.Web
{
	/// <summary>
	/// A wrapper for web methods and properties available through 
	/// <see cref="System.Web.HttpContext.Current"/>.
	/// </summary>
	public class WebContextWrapper : IWebContext
	{
		public HttpContext CurrentHttpContext
		{
			get { return HttpContext.Current; }
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
		public virtual IHttpHandler CurrentHandler 
		{
			get { return IsInWebContext ? CurrentHttpContext.Handler : null; }
		}

		/// <summary>Gets the current host name.</summary>
		public virtual string CurrentHost
		{
			get { return IsInWebContext ? CurrentHttpContext.Request.Url.Authority : null; }
		}

		/// <summary>Gets the current user in the web execution context.</summary>
		public virtual IPrincipal CurrentUser
		{
			get { return IsInWebContext ? CurrentHttpContext.User : null; }
		}

		/// <summary>The current request object.</summary>
		public virtual HttpRequest Request
		{
			get { return CurrentHttpContext != null ? CurrentHttpContext.Request : null; }
		}

		/// <summary>The current request object.</summary>
		public virtual HttpResponse Response
		{
			get { return CurrentHttpContext != null ? CurrentHttpContext.Response : null; }
		}

		/// <summary>Gets the url relative to the application root.</summary>
		public virtual string RelativeUrl
		{
			get { return Request.AppRelativeCurrentExecutionFilePath; }
		}

		/// <summary>Gets the application root path.</summary>
		public string ApplicationUrl
		{
			get { return Request.ApplicationPath; }
		}

		/// <summary>Gets the current request's query string.</summary>
		public string QueryString
		{
			get { return Request.QueryString.ToString(); }
		}


		/// <summary>Converts a virtual url to an absolute url.</summary>
		/// <param name="virtualPath">The virtual url to make absolute.</param>
		/// <returns>The absolute url.</returns>
		public virtual string ToAbsolute(string virtualPath)
		{
			return VirtualPathUtility.ToAbsolute(virtualPath);
		}

		/// <summary>Converts an absolute url to an app relative url.</summary>
		/// <param name="virtualPath">The absolute url to convert.</param>
		/// <returns>An app relative url.</returns>
		public virtual string ToAppRelative(string virtualPath)
		{
			return VirtualPathUtility.ToAppRelative(virtualPath);
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
		public void RewritePath(string path)
		{
			CurrentHttpContext.RewritePath(path, false);
		}
	}
}
