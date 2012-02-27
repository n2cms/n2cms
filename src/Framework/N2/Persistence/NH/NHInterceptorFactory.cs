using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;
using N2.Persistence.Proxying;
using log4net;
using NHibernate;

namespace N2.Persistence.NH
{
	[Service]
	public class NHInterceptorFactory
	{
		private readonly IProxyFactory interceptor;
		private readonly IItemNotifier notifier;
		private readonly ILog logger = LogManager.GetLogger(typeof(NHInterceptorFactory));

		public NHInterceptorFactory(IProxyFactory interceptor, IItemNotifier notifier)
		{
			this.interceptor = interceptor;
			this.notifier = notifier;
		}

		public virtual ISession CreateSession(ISessionFactory sessionFactory)
		{
			var nhi = new NHInterceptor(interceptor, notifier);
			var s = sessionFactory.OpenSession(nhi);
			nhi.Session = s;
			return s;
		}
	}
}
