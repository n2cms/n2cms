using N2.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.Xml
{
	[Service(typeof(LinqQueryFacade), Configuration = "xml", Replaces = typeof(LinqQueryFacade))]
	public class LinqXmlQueryFacade : LinqQueryFacade
	{
		private IEngine engine;

		public LinqXmlQueryFacade(IEngine engine)
		{
			this.engine = engine;
		}

		public override IQueryable<T> Query<T>()
		{
			return engine.Resolve<IRepository<T>>().Find().AsQueryable();
		}
	}
}
