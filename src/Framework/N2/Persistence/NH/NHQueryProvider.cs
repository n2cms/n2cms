using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Linq;
using N2.Engine;

namespace N2.Persistence.NH
{
	[Service(typeof(LinqQueryProvider))]
	public class NHQueryProvider : LinqQueryProvider
	{
		private ISessionProvider session;
		
		public NHQueryProvider(ISessionProvider session)
		{
			this.session = session;
		}
		
		public override IQueryable<T> Query<T>()
		{
			return session.OpenSession.Session.Query<T>();
		}
	}
}
