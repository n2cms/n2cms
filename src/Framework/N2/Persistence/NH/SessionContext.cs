using N2.Web;
using NHibernate;

namespace N2.Persistence.NH
{
    /// <summary>
    /// A wrapper class that supports the <see cref="IClosable"/> interface 
    /// which means it will be disposed at the end of the request.
    /// </summary>
    public class SessionContext : IClosable
    {
        private ISession session;
        private ISessionProvider provider;

        public ISession Session
        {
            get { return session; }
            set { session = value; }
        }

        public SessionContext(ISessionProvider provider, ISession session)
        {
            this.provider = provider;
            this.session = session;
        }

		public System.Data.IDbConnection Connection
		{
			get { return Session.Connection; }
		}

        #region IDisposable Members

        public void Dispose()
        {
            provider.Dispose();
        }

        #endregion
    }
}
