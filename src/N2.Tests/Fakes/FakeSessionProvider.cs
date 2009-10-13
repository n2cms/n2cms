using System;
using System.Data;
using N2.Persistence;
using N2.Persistence.NH;
using N2.Web;
using NHibernate;

namespace N2.Tests.Fakes
{
	public class FakeSessionProvider : SessionProvider, IDisposable
	{
		ISession session;
		IItemNotifier interceptor;

		public FakeSessionProvider(IConfigurationBuilder builder, IItemNotifier interceptor, IWebContext webContext)
			: base(builder, interceptor, webContext)
		{
			this.interceptor = interceptor;
		}

		public override SessionContext OpenSession
		{
			get
			{
				if(session == null)
					session = NHSessionFactory.OpenSession(interceptor);
				return CurrentSession ?? (CurrentSession = new SessionContext(this, session));
			}
		}

		public void Dispose()
		{
			if(session != null)
				session.Clear();
		}

		public void CloseConnections()
		{
			if(session != null)
			{
				session.Dispose();
				session = null;
				CurrentSession = null;
			}
		}
	}
}
