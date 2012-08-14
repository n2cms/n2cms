using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;
using N2.Persistence.Proxying;
using NHibernate;

namespace N2.Persistence.NH
{
	[Service]
	public class NHInterceptorFactory
	{
		private readonly IProxyFactory interceptor;
		private readonly IItemNotifier notifier;
		private readonly Engine.Logger<NHInterceptorFactory> logger;

		public NHInterceptorFactory(IProxyFactory interceptor, IItemNotifier notifier)
		{
			this.interceptor = interceptor;
			this.notifier = notifier;
		}

		public virtual ISession CreateSession(ISessionFactory sessionFactory)
		{
			logger.Debug("Creating NH session");

			var nhi = new NHInterceptor(interceptor, notifier);
			var s = sessionFactory.OpenSession(nhi);
			nhi.Session = s;
			return s;
		}
	}
}
