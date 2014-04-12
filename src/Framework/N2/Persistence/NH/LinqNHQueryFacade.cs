using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Linq;
using N2.Engine;

namespace N2.Persistence.NH
{
	[Service(typeof(LinqQueryFacade))]
	public class LinqNHQueryFacade : LinqQueryFacade
	{
		private ISessionProvider session;
		
		public LinqNHQueryFacade(ISessionProvider session)
		{
			this.session = session;
		}
		
		public override IQueryable<T> Query<T>()
		{
			return session.OpenSession.Session.Query<T>();
		}
	}
}
