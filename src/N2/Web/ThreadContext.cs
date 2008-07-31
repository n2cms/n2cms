using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Collections;
using System.Security.Principal;
using System.IO;
using System.Collections.Specialized;

namespace N2.Web
{
	public class ThreadContext : RequestContext, IDisposable
	{
		[ThreadStatic]
		private static IDictionary items;
        [ThreadStatic]
        private static NameValueCollection queryString;
		static string baseDirectory;

		static ThreadContext()
		{
			baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
			int binIndex = baseDirectory.IndexOf("\\bin\\");
			if (binIndex >= 0)
				baseDirectory = baseDirectory.Substring(0, binIndex);
			else if (baseDirectory.EndsWith("\\bin"))
				baseDirectory = baseDirectory.Substring(0, baseDirectory.Length - 4);
        }

        public override System.Web.IHttpHandler Handler
        {
            get { throw new NotSupportedException("In thread context. No handler when not running in http web context."); }
        }

		public override IDictionary RequestItems
		{
            get { return items ?? (items = new Hashtable()); }
		}

		public override NameValueCollection QueryString
		{
			get { return queryString ?? (queryString = new NameValueCollection()); }
		}

		public override IPrincipal User
		{
			get { return Thread.CurrentPrincipal; }
		}

		public override string MapPath(string path)
		{
			path = path.Replace("~/", "").TrimStart('/').Replace('/', '\\');
			return Path.Combine(baseDirectory, path);
		}

        public override System.Web.HttpRequest Request
        {
            get { throw new NotSupportedException("In thread context. No handler when not running in http web context."); }
        }

        public override System.Web.HttpResponse Response
        {
            get { throw new NotSupportedException("In thread context. No handler when not running in http web context."); }
        }

        public override System.Web.HttpCookieCollection Cookies
        {
            get { throw new NotSupportedException("In thread context. No handler when not running in http web context."); }
        }

        public override void RewritePath(string path)
        {
            throw new NotSupportedException("In thread context. No handler when not running in http web context.");
        }

        #region IDisposable Members

        public void Dispose()
        {
            items = null;
            queryString = null;
        }

        #endregion
    }
}
