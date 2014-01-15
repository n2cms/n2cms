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
        Logger<SessionProvider> logger;
        private static string SessionKey = "SessionProvider.Session";
        private NHInterceptorFactory interceptorFactory;
        private readonly IWebContext webContext;
        private readonly ISessionFactory nhSessionFactory;
        private FlushMode flushAt = FlushMode.Commit;
        private System.Data.IsolationLevel? isolation;
        private bool autoStartTransaction;

        public System.Data.IsolationLevel? Isolation
        {
            get { return isolation; }
            set { isolation = value; }
        }

        public SessionProvider(IConfigurationBuilder builder, NHInterceptorFactory interceptorFactory, IWebContext webContext, DatabaseSection config)
        {
            if (config.Flavour.IsFlagSet(DatabaseFlavour.NoSql))
                return;

            nhSessionFactory = builder.BuildSessionFactory();
            this.webContext = webContext;
            this.interceptorFactory = interceptorFactory;
            this.isolation = config.Isolation;
            this.autoStartTransaction = config.AutoStartTransaction;
            this.CacheEnabled = config.Caching;
        }

        /// <summary>Tells whether cache should be enabled by default.</summary>
        public bool CacheEnabled { get; set; }
        /// <summary>Gets the NHibernate session factory</summary>
        public ISessionFactory SessionFactory
        {
            get { return nhSessionFactory; }
        }

        /// <summary>Gets an existing session context or null if no session is in progress.</summary>
        public virtual SessionContext CurrentSession
        {
            get { return webContext == null ? null : webContext.RequestItems[SessionKey] as SessionContext; }
            set { if (webContext != null) webContext.RequestItems[SessionKey] = value; }
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
                if (sc == null)
                {
                    ISession s = interceptorFactory.CreateSession(nhSessionFactory);
                    s.FlushMode = FlushAt;
                    CurrentSession = sc = new SessionContext(this, s);
                    logger.DebugFormat("Creating session {0}", sc.GetHashCode());
                    if (autoStartTransaction)
                        sc.Transaction = BeginTransaction();
                }
                else
                {
                    logger.DebugFormat("Reusing session {0}", sc.GetHashCode());
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
                if (autoStartTransaction && sc.Transaction != null)
                    sc.Transaction.Commit();

                logger.DebugFormat("Closing session {0}", sc.GetHashCode());
                sc.Session.Dispose();
                CurrentSession = null;
            }
        }

        /// <summary>Begins a transaction.</summary>
        /// <returns>A disposable transaction wrapper. Call Commit to commit the transaction.</returns>
        public ITransaction BeginTransaction()
        {
            var transaction = new NHTransaction(this);
            if (transaction.IsCommitted || transaction.IsRollbacked)
            {
                Debug.WriteLine("Ending previous transaction");
                transaction.Dispose();
                CurrentSession.Transaction = null;
            }
            if (CurrentSession.Transaction == null)
                CurrentSession.Transaction = transaction;
            return transaction;
        }

        /// <summary>Gets an existing transaction or null if no transaction has been initiated.</summary>
        /// <returns>A disposable transaction wrapper. Call Commit to commit the transaction.</returns>
        public ITransaction GetTransaction()
        {
            if (CurrentSession == null)
                return null;
            if (CurrentSession.Transaction == null)
                return null;

            return new SubTransaction(CurrentSession.Transaction);
        }
    }
}
