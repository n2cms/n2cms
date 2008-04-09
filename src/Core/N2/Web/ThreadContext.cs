using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Collections;
using System.Security.Principal;
using System.IO;

namespace N2.Web
{
	public class ThreadContext : RequestContext
	{
		[ThreadStatic]
		private static IDictionary items = new Hashtable();
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
