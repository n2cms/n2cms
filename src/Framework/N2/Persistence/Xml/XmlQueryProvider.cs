using N2.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.Xml
{
	[Service(typeof(LinqQueryProvider), Configuration = "xml")]
	public class XmlQueryProvider : LinqQueryProvider
	{
		private IEngine engine;

		public XmlQueryProvider(IEngine engine)
		{
			this.engine = engine;
		}

		public override IQueryable<T> Query<T>()
		{
			return engine.Resolve<IRepository<T>>().Find().AsQueryable();
		}
	}
}
