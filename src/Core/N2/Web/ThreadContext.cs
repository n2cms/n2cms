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
	public class ThreadContext : RequestContext
	{
		[ThreadStatic]
		private static IDictionary items = new Hashtable();
		[ThreadStatic]
		private static NameValueCollection queryString = new NameValueCollection();
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

		public override IDictionary RequestItems
		{
			get { return items; }
		}

		public override NameValueCollection QueryString
		{
			get { return queryString; }
		}

		public override IPrincipal User
		{
			get { return Thread.CurrentPrincipal; }
		}

		public override string MapPath(string path)
		{
			path = path.Replace("~/", ".\\").Replace('/', '\\');
			return Path.Combine(baseDirectory, path);
		}
	}
}
