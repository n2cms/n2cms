using System;
using System.Diagnostics;
using N2.Engine;
using N2.Web;
using NHibernate;

namespace N2.Persistence.NH
{
	/// <summary>
	/// Provides access to opened nhibernate sessions.
	/// </summary>
	[Service(typeof(ISessionProvider))]
	public class SessionProvider : ISessionProvider
	{
		private static string RequestItemsKey = "SessionProvider.Session";
		private IInterceptor interceptor;
		private readonly IWebContext webContext;
		private readonly ISessionFactory nhSessionFactory;
        private FlushMode flushAt = FlushMode.Commit;

		public SessionProvider(IConfigurationBuilder builder, IInterceptor interceptor, IWebContext webContext)
		{
            nhSessionFactory = builder.BuildSessionFactory();
            Debug.WriteLine("Built Session Factory " + DateTime.Now);
            this.webContext = webContext;
            this.interceptor = interceptor;
		}

		public IInterceptor Interceptor
		{
			get { return interceptor; }
		}

		/// <summary>Gets the NHibernate session factory</summary>
		public ISessionFactory NHSessionFactory
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

        public virtual SessionContext CreateSession
        {
            get
            {
                ISession s = nhSessionFactory.OpenSession(Interceptor);
                s.FlushMode = FlushAt;
                return CurrentSession = new SessionContext(this, s);
            }
        }

		public virtual SessionContext OpenSession
		{
            get
            {
                SessionContext sc = CurrentSession;
                if(sc == null)
                {
                    sc = CreateSession;
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
	}
}