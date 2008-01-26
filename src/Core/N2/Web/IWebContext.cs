using System;
using System.Security.Principal;
using System.Collections;
using System.Web;

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

		/// <summary>Gets the current host name, e.g. www.n2cms.com.</summary>
		string CurrentHost { get; }

		/// <summary>Gets the current user principal (may be null).</summary>
		IPrincipal CurrentUser { get; }

		/// <summary>The current request object.</summary>
		HttpRequest Request { get; }

		/// <summary>The current response object.</summary>
		HttpResponse Response { get; }

		/// <summary>Gets the url relative to the application root.</summary>
		string RelativeUrl { get; }

		/// <summary>Gets the application root path.</summary>
		string ApplicationUrl { get; }

		/// <summary>Gets the current request's query string.</summary>
		string QueryString { get; }

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
