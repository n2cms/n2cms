using System;
using System.Security.Principal;
using System.Collections;
using System.Web;
using System.Collections.Specialized;

namespace N2.Web
{
	/// <summary>
	/// A mockable wrapper for operations on the http context.
	/// </summary>
	public interface IWebContext
	{
		/// <summary>Gets wether there is a web context availabe.</summary>
		bool IsInWebContext { get; }

		/// <summary>Gets a dictionary of request scoped items.</summary>
		IDictionary RequestItems { get; }

		/// <summary>Gets the current user principal (may be null).</summary>
		IPrincipal User { get; }

		/// <summary>The current request object.</summary>
		HttpRequest Request { get; }

		/// <summary>The current response object.</summary>
		HttpResponse Response { get; }

		/// <summary>The handler associated with this request.</summary>
		IHttpHandler Handler { get; }

		/// <summary>A page instance stored in the request context.</summary>
		ContentItem CurrentPage { get; set; }

		/// <summary>Gets the current host name, e.g. www.n2cms.com.</summary>
		string Host { get; }

		/// <summary>Gets the url relative to the application root.</summary>
		string RawUrl { get; }

		/// <summary>The absolute path to the requested page without query string.</summary>
		string AbsolutePath { get; }

		/// <summary>The request's cookie collection.</summary>
		HttpCookieCollection Cookies { get; }

		/// <summary>Gets the application root path.</summary>
		string ApplicationUrl { get; }

		/// <summary>Gets the current request's query string.</summary>
		string Query { get; }

		/// <summary>Gets the current request's query string.</summary>
		NameValueCollection QueryString { get; }

		/// <summary>The physical path on disk to the requested page.</summary>
		string PhysicalPath { get; }

		/// <summary>Converts a virtual path to an an absolute path. E.g. ~/hello.aspx -> /MyVirtualDirectory/hello.aspx.</summary>
		/// <param name="virtualPath">The virtual url to make absolute.</param>
		/// <returns>The absolute url.</returns>
		string ToAbsolute(string virtualPath);
		
		/// <summary>Converts an absolute url to an app relative path. E.g. /MyVirtualDirectory/hello.aspx -> ~/hello.aspx.</summary>
		/// <param name="virtualPath">The absolute url to convert.</param>
		/// <returns>An app relative url.</returns>
		string ToAppRelative(string virtualPath);

		/// <summary>Maps a virtual path to a physical disk path.</summary>
		/// <param name="path">The path to map. E.g. "~/bin"</param>
		/// <returns>The physical path. E.g. "c:\inetpub\wwwroot\bin"</returns>
		string MapPath(string path);

		/// <summary>Assigns a rewrite path.</summary>
		/// <param name="path">The path to the template that will handle the request.</param>
		void RewritePath(string path);
	}
}
