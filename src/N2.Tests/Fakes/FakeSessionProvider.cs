using System;
using System.Data;
using N2.Persistence.NH;
using N2.Web;
using NHibernate;

namespace N2.Tests.Fakes
{
	public class FakeSessionProvider : SessionProvider, IDisposable
	{
		ISession session;
		
		public FakeSessionProvider(IConfigurationBuilder builder, IInterceptor interceptor, IWebContext webContext)
			: base(builder, interceptor, webContext)
		{
		}

		public override SessionContext OpenSession
		{
			get
			{
				if(session == null)
					session = NHSessionFactory.OpenSession();
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
