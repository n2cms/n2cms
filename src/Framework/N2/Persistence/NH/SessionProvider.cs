using System;
using System.Diagnostics;
using N2.Engine;
using N2.Web;
using NHibernate;
using log4net;
using N2.Configuration;

namespace N2.Persistence.NH
{
	/// <summary>
	/// Provides access to opened nhibernate sessions.
	/// </summary>
	[Service(typeof(ISessionProvider))]
	public class SessionProvider : ISessionProvider
	{
		private readonly ILog logger = LogManager.GetLogger(typeof(SessionProvider));

		private static string RequestItemsKey = "SessionProvider.Session";
		private NHInterceptorFactory interceptorFactory;
		private readonly IWebContext webContext;
		private readonly ISessionFactory nhSessionFactory;
        private FlushMode flushAt = FlushMode.Commit;
		private System.Data.IsolationLevel? isolation;

		public SessionProvider(IConfigurationBuilder builder, NHInterceptorFactory interceptorFactory, IWebContext webContext, DatabaseSection config)
		{
			nhSessionFactory = builder.BuildSessionFactory();
			logger.Debug("Built Session Factory " + DateTime.Now);
			this.webContext = webContext;
			this.interceptorFactory = interceptorFactory;
			this.isolation = config.Isolation;
			this.CacheEnabled = config.Caching;
		}

		/// <summary>Tells whether cache should be enabled by default.</summary>
		public bool CacheEnabled { get; set; }
		/// <summary>Gets the NHibernate session factory</summary>
		public ISessionFactory SessionFactory
		{
			get { return nhSessionFactory; }
		}

		public virtual SessionContext CurrentSession
		{
			get { return webContext.RequestItems[RequestItemsKey] as SessionContext; }
			set { webContext.RequestItems[RequestItemsKey] = value; }
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

		public ITransaction BeginTransaction()
		{
			return new NHTransaction(isolation, this);
		}
	}
}