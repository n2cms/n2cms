using System;
using System.Collections.Generic;
using System.Text;
using N2.Web;
using NHibernate;

namespace N2.Persistence.NH
{
    public class SessionContext : IClosable
    {
        private ISession session;

        public ISession Session
        {
            get { return session; }
            set { session = value; }
        }
        ISessionProvider provider;

        public SessionContext(ISessionProvider provider, ISession session)
        {
            this.provider = provider;
            this.session = session;
        }

        #region IDisposable Members

        public void Dispose()
        {
            provider.Dispose();
        }

        #endregion
    }
}
