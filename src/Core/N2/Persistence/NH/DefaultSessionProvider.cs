using System.Diagnostics;
using N2.Web;
using NHibernate;

namespace N2.Persistence.NH
{
	/// <summary>
	/// Provides access to opened nhibernate sessions.
	/// </summary>
	public class DefaultSessionProvider : ISessionProvider
	{
		private static string RequestItemsKey = "SessionProvider.Session";
		private IInterceptor interceptor;
		private readonly IWebContext webContext;
		private readonly ISessionFactory nhSessionFactory;
		
		public DefaultSessionProvider(IConfigurationBuilder builder, IInterceptor interceptor, IWebContext webContext)
			: this(builder, webContext)
		{
			this.interceptor = interceptor;
		}

		public DefaultSessionProvider(IConfigurationBuilder builder, IWebContext webContext)
		{
			nhSessionFactory = builder.BuildSessionFactory();
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
			get { return webContext.RequestItems[RequestItemsKey] as ISession; }
			set { webContext.RequestItems[RequestItemsKey] = value; }
		}

		#region ISessionProvider Members

		public virtual ISession GetOpenedSession()
		{
			if (Session == null)
			{
				Session = OpenSessionWithOptionalInterceptor();
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

		#endregion

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