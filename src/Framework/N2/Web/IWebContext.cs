using System;
using System.Collections;
using System.Security.Principal;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;

namespace N2.Web
{
    /// <summary>
    /// A mockable interface for operations that targets the the http context.
    /// </summary>
    public interface IWebContext
    {
        /// <summary>Gets wether there is a web context availabe.</summary>
        bool IsWeb { get; }

        /// <summary>Gets a dictionary of request scoped items.</summary>
        IDictionary RequestItems { get; }

        /// <summary>Gets the current user principal (may be null).</summary>
        IPrincipal User { get; }

        /// <summary>The http context associated with the current request (if any).</summary>
        HttpContextBase HttpContext { get; }

        /// <summary>A page instance stored in the request context.</summary>
        ContentItem CurrentPage { get; set; }

        /// <summary>The template used to serve this request.</summary>
        PathData CurrentPath { get; set; }

        /// <summary>The local part of the requested path, e.g. http://n2cms.com/path/to/a/page.aspx?some=query.</summary>
        Url Url { get; }

        /// <summary>Closes any endable resources at the end of the request.</summary>
        void Close();

        /// <summary>Maps a virtual path to a physical disk path.</summary>
        /// <param name="path">The path to map. E.g. "~/bin"</param>
        /// <returns>The physical path. E.g. "c:\inetpub\wwwroot\bin"</returns>
        string MapPath(string path);

        /// <summary>The current request object.</summary>
        [Obsolete("Use HttpContext.Request")]
        HttpRequest Request { get; }

        /// <summary>The current response object.</summary>
        [Obsolete("Use HttpContext.Response")]
        HttpResponse Response { get; }

        /// <summary>The handler associated with this request.</summary>
        [Obsolete("Use HttpContext.Handler")]
        IHttpHandler Handler { get; }

        /// <summary>The physical path on disk to the requested page.</summary>
        [Obsolete("Use HttpContext.Request.PhysicalPath")]
        string PhysicalPath { get; }

        /// <summary>
        /// Specifies whether the UrlAuthorizationModule should skip authorization for the current request.
        /// </summary>
        [Obsolete("Use HttpContext.SkipAuthorization")]
        bool SkipAuthorization { get; }

        /// <summary>Converts a virtual path to an an absolute path. E.g. ~/hello.aspx -> /MyVirtualDirectory/hello.aspx.</summary>
        /// <param name="virtualPath">The virtual url to make absolute.</param>
        /// <returns>The absolute url.</returns>
        [Obsolete("Use N2.Web.Url.ToAbsolute(path)")]
        string ToAbsolute(string virtualPath);
        
        /// <summary>Converts an absolute url to an app relative path. E.g. /MyVirtualDirectory/hello.aspx -> ~/hello.aspx.</summary>
        /// <param name="virtualPath">The absolute url to convert.</param>
        /// <returns>An app relative url.</returns>
        [Obsolete("Use N2.Web.Url.ToRelative(path)")]
        string ToAppRelative(string virtualPath);

        /// <summary>Rewrites the request to the given path.</summary>
        /// <param name="path">The path to the template that will handle the request.</param>
        [Obsolete("Use HttpContext.RewritePath(path)")]
        void RewritePath(string path);

        /// <summary>Transferes the request to the given path.</summary>
        /// <param name="path">The path to the template that will handle the request.</param>
        /// <param name="queryString">The query string to rewrite to.</param>
        [Obsolete("Use HttpContext.RewritePath(path, \"\", queryString)")]
        void RewritePath(string path, string queryString);

        /// <summary>Calls into HttpContext.ClearError().</summary>
        [Obsolete("Use HttpContext.ClearError()")]
        void ClearError();

        /// <summary>Retrieves the http context cache.</summary>
        Cache Cache { get; }

        /// <summary>Virtual path provider.</summary>
        VirtualPathProvider Vpp { get; }
    }
}
