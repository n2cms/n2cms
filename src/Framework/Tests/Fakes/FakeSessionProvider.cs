using System;
using N2.Persistence.NH;
using N2.Web;
using NHibernate;

namespace N2.Tests.Fakes
{
	public class FakeSessionProvider : SessionProvider, IDisposable
	{
		ISession session;
		IInterceptor interceptor;

		public FakeSessionProvider(IConfigurationBuilder builder, IInterceptor interceptor, IWebContext webContext)
			: base(builder, interceptor, webContext, new N2.Configuration.DatabaseSection())
		{
			this.interceptor = interceptor;
		}

		public override SessionContext OpenSession
		{
			get
			{
				if(session == null)
					session = SessionFactory.OpenSession(interceptor);
				return CurrentSession ?? (CurrentSession = new SessionContext(this, session));
			}
		}

		public override void Dispose()
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
