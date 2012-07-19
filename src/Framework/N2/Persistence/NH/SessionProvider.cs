using System;
using System.Diagnostics;
using N2.Engine;
using N2.Web;
using NHibernate;
using N2.Configuration;

namespace N2.Persistence.NH
{
	/// <summary>
	/// Provides access to opened nhibernate sessions.
	/// </summary>
	[Service(typeof(ISessionProvider))]
	public class SessionProvider : ISessionProvider
	{
		private static string SessionKey = "SessionProvider.Session";
		private NHInterceptorFactory interceptorFactory;
		private readonly IWebContext webContext;
		private readonly ISessionFactory nhSessionFactory;
        private FlushMode flushAt = FlushMode.Commit;
		private System.Data.IsolationLevel? isolation;

		public SessionProvider(IConfigurationBuilder builder, NHInterceptorFactory interceptorFactory, IWebContext webContext, DatabaseSection config)
		{
			nhSessionFactory = builder.BuildSessionFactory();
			this.webContext = webContext;
			this.interceptorFactory = interceptorFactory;
			this.isolation = config.Isolation;
		}

		/// <summary>Gets the NHibernate session factory</summary>
		public ISessionFactory SessionFactory
		{
			get { return nhSessionFactory; }
		}

		/// <summary>Gets an existing session context or null if no session is in progress.</summary>
		public virtual SessionContext CurrentSession
		{
			get { return webContext.RequestItems[SessionKey] as SessionContext; }
			set { webContext.RequestItems[SessionKey] = value; }
		}

        public FlushMode FlushAt
        {
            get { return flushAt; }
            set { flushAt = value; }
        }

		public virtual SessionContext OpenSession
		{
            get
            {
                SessionContext sc = CurrentSession;
                if(sc == null)
                {
					ISession s = interceptorFactory.CreateSession(nhSessionFactory);
				    s.FlushMode = FlushAt;
                    CurrentSession = sc = new SessionContext(this, s);
                }
                return sc;
            }
		}

	    public virtual void Flush()
		{
            SessionContext sc = CurrentSession;

            if (sc != null)
				sc.Session.Flush();
		}

		public virtual void Dispose()
		{
            SessionContext sc = CurrentSession;

            if (sc != null)
            {
                sc.Session.Dispose();
                CurrentSession = null;
            }
        }

		/// <summary>Begins a transaction.</summary>
		/// <returns>A disposable transaction wrapper. Call Commit to commit the transaction.</returns>
		public ITransaction BeginTransaction()
		{
			var transaction = new NHTransaction(isolation, this);
			if (CurrentSession.Transaction == null)
				CurrentSession.Transaction = transaction;
			return transaction;
		}

		/// <summary>Gets an existing transaction or null if no transaction has been initiated.</summary>
		/// <returns>A disposable transaction wrapper. Call Commit to commit the transaction.</returns>
		public ITransaction GetTransaction()
		{
			return CurrentSession != null ? CurrentSession.Transaction : null;
		}
	}
}