using System.Collections;
using System.Security.Principal;
using System.Web;
using System.Diagnostics;
using System.Collections.Specialized;
using System;

namespace N2.Web
{
	/// <summary>
	/// A wrapper for web methods and properties available through 
	/// <see cref="System.Web.HttpContext.Current"/>.
	/// </summary>
	public abstract class RequestContext : IWebContext
	{
		/// <summary>Gets wether there is a web context availabe.</summary>
		public virtual bool IsWeb
		{
            get { return HttpContext.Current != null; }
		}

        public abstract IDictionary RequestItems { get; }
        public abstract IHttpHandler Handler { get; }

		/// <summary>Gets the current host name.</summary>
		public virtual string Authority
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
				return Url.PathPart(RawUrl);
			}
		}

		/// <summary>The physical path on disk to the requested resource.</summary>
		public virtual string PhysicalPath
		{
			get { return Request.PhysicalPath; }
		}

		/// <summary>A page instance stored in the request context.</summary>
		public ContentItem CurrentPage
		{
			get { return RequestItems["CurrentPage"] as ContentItem; }
			set { RequestItems["CurrentPage"] = value; }
		}

		/// <summary>Converts a virtual url to an absolute url.</summary>
		/// <param name="virtualPath">The virtual url to make absolute.</param>
		/// <returns>The absolute url.</returns>
		public virtual string ToAbsolute(string virtualPath)
		{
			return N2.Web.Url.ToAbsolute(virtualPath);
		}

		/// <summary>Converts an absolute url to an app relative url.</summary>
		/// <param name="virtualPath">The absolute url to convert.</param>
		/// <returns>An app relative url.</returns>
		public virtual string ToAppRelative(string virtualPath)
		{
			if (virtualPath != null && virtualPath.StartsWith(Url.ApplicationPath, System.StringComparison.InvariantCultureIgnoreCase))
				return "~/" + virtualPath.Substring(Url.ApplicationPath.Length);
			return virtualPath;
		}

		/// <summary>Maps a virtual path to a physical disk path.</summary>
		/// <param name="path">The path to map. E.g. "~/bin"</param>
		/// <returns>The physical path. E.g. "c:\inetpub\wwwroot\bin"</returns>
        public abstract string MapPath(string path);

		/// <summary>Assigns a rewrite path.</summary>
		/// <param name="path">The path to the template that will handle the request.</param>
        public abstract void RewritePath(string path);

        public abstract IPrincipal User { get; }

        public abstract HttpRequest Request { get; }

        public abstract HttpResponse Response { get; }

        public abstract HttpCookieCollection Cookies { get; }

        public virtual void Dispose()
        {
            string[] keys = new string[RequestItems.Keys.Count];
            RequestItems.Keys.CopyTo(keys, 0);

            foreach (string key in keys)
            {
                IClosable value = RequestItems[key] as IClosable;
                if (value != null)
                {
                    (value as IClosable).Dispose();
                }
            }
        }
    }
}
