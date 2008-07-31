using System.Diagnostics;
using N2.Web;
using NHibernate;
using System;

namespace N2.Persistence.NH
{
	/// <summary>
	/// Provides access to opened nhibernate sessions.
	/// </summary>
	public class SessionProvider : ISessionProvider
	{
		private static string RequestItemsKey = "SessionProvider.Session";
		private IInterceptor interceptor;
		private readonly IWebContext webContext;
		private readonly ISessionFactory nhSessionFactory;
		
		public SessionProvider(IConfigurationBuilder builder, IInterceptor interceptor, IWebContext webContext)
			: this(builder, webContext)
		{
			this.interceptor = interceptor;
		}

		public SessionProvider(IConfigurationBuilder builder, IWebContext webContext)
		{
			nhSessionFactory = builder.BuildSessionFactory();
			Debug.WriteLine("Built Session Factory " + DateTime.Now);
			this.webContext = webContext;
		}

		public IInterceptor Interceptor
		{
			get { return interceptor; }
			set { interceptor = value; }
		}

		/// <summary>Gets the NHibernate session factory</summary>
		public ISessionFactory NHSessionFactory
		{
			get { return nhSessionFactory; }
		}

		public virtual ISession Session
		{
			get 
            {
                SessionContext ctx = webContext.RequestItems[RequestItemsKey] as SessionContext;
                if (ctx != null)
                    return ctx.Session;
                return null;
            }
			set { webContext.RequestItems[RequestItemsKey] = new SessionContext(this, value); }
		}

		public virtual ISession GetOpenedSession()
		{
			if (Session == null)
			{
				ISession s = OpenSessionWithOptionalInterceptor();
				s.FlushMode = FlushMode.Commit;
				Session = s;
			}
			return Session;
		}

		public virtual void Flush()
		{
			if (Session != null)
				Session.Flush();
		}

		public void Dispose()
		{
			EndSession(false);
		}

		protected virtual ISession OpenSessionWithOptionalInterceptor()
		{
			if (Interceptor == null)
				return nhSessionFactory.OpenSession();
			else
				return nhSessionFactory.OpenSession(Interceptor);
		}

		public virtual void EndSession(bool flush)
		{
			ISession s = Session;

			if (s != null)
			{
				if (flush)
					s.Flush();
				s.Dispose();
				Session = null;
			}
		}
	}
}