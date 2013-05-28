using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using N2.Engine;
using N2.Persistence.NH;

namespace N2.Persistence.Serialization.Html
{
	[Service(typeof (ISessionProvider), Configuration = "xml", Replaces = typeof (SessionProvider))]
	internal class XmlSessionProvider : ISessionProvider
	{
		public bool CacheEnabled
		{
			get { return false; }
		}

		public System.Data.IsolationLevel? Isolation
		{
			get { return IsolationLevel.Unspecified; }
		}

		public void Flush()
		{
		}

		public ITransaction BeginTransaction()
		{
			return null;
		}

		public ITransaction GetTransaction()
		{
			return null;
		}

		public void Dispose()
		{
		}


		public SessionContext OpenSession
		{
			get { return null; }
		}


		public NHibernate.ISessionFactory SessionFactory
		{
			get { return null; }
		}
	}
}
