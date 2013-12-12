using System;
using N2.Persistence.NH;
using N2.Web;
using NHibernate;

namespace N2.Tests.Fakes
{
    public class FakeSessionProvider : SessionProvider, IDisposable
    {
        private ISession session;
        private NHInterceptorFactory factory;

        public FakeSessionProvider(IConfigurationBuilder builder, NHInterceptorFactory factory, IWebContext webContext)
            : base(builder, factory, webContext, new N2.Configuration.DatabaseSection())
        {
            this.factory = factory;
        }

        public override SessionContext OpenSession
        {
            get
            {
                if (session == null)
                    session = factory.CreateSession(SessionFactory);
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
