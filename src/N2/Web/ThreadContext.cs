using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Collections;
using System.Security.Principal;
using System.IO;
using System.Collections.Specialized;
using System.Web;

namespace N2.Web
{
    /// <summary>
    /// A thread local context. It's used to store the nhibernate session 
    /// instance in situations where we don't have a request available such
    /// as code executed by the scheduler.
    /// </summary>
	public class ThreadContext : IWebContext
    {
        private static string baseDirectory;

    	[ThreadStatic] ContentItem currentPage;
    	[ThreadStatic] TemplateData currentTemplate;
    	[ThreadStatic] static IDictionary items;
    	[ThreadStatic] Url localUrl = new Url("/");
    	[ThreadStatic] Url hostUrl = new Url("http://localhost");

        static ThreadContext()
		{
			baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
			int binIndex = baseDirectory.IndexOf("\\bin\\");
			if (binIndex >= 0)
				baseDirectory = baseDirectory.Substring(0, binIndex);
			else if (baseDirectory.EndsWith("\\bin"))
				baseDirectory = baseDirectory.Substring(0, baseDirectory.Length - 4);
        }


		public virtual IDictionary RequestItems
		{
            get { return items ?? (items = new Hashtable()); }
		}

		public virtual IPrincipal User
		{
			get { return Thread.CurrentPrincipal; }
		}

        public bool IsWeb
        {
            get { return false; }
        }

        public ContentItem CurrentPage
        {
            get { return currentPage; }
            set { currentPage = value; }
		}

		/// <summary>The template used to serve this request.</summary>
		public TemplateData CurrentTemplate
		{
			get { return currentTemplate; }
			set { currentTemplate = value; }
		}

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
            items = null;
        }

        public virtual Url LocalUrl
        {
            get { return localUrl; }
            set { localUrl = value; }
        }
        
        public virtual Url HostUrl
        {
            get { return hostUrl; }
            set { hostUrl = value; }
        }

        public virtual string MapPath(string path)
        {
            path = path.Replace("~/", "").TrimStart('/').Replace('/', '\\');
            return Path.Combine(baseDirectory, path);
        }

        public virtual IHttpHandler Handler
        {
            get { throw new NotSupportedException("In thread context. No handler when not running in http web context."); }
        }

        public virtual HttpRequest Request
        {
            get { throw new NotSupportedException("In thread context. No handler when not running in http web context."); }
        }

        public virtual HttpResponse Response
        {
            get { throw new NotSupportedException("In thread context. No handler when not running in http web context."); }
        }

        public virtual string PhysicalPath
        {
			get { return MapPath(LocalUrl.Path); }
        }

        public virtual void RewritePath(string path)
        {
			throw new NotSupportedException("RewritePath not supported in thread context. No handler when not running in http web context.");
        }

        public virtual string ToAbsolute(string virtualPath)
        {
            throw new NotSupportedException("In thread context. No handler when not running in http web context.");
        }

        public virtual string ToAppRelative(string virtualPath)
        {
            throw new NotSupportedException("In thread context. No handler when not running in http web context.");
        }

        public void TransferRequest(string path)
        {
            throw new NotSupportedException("In thread context. No handler when not running in http web context.");
        }
    }
}
