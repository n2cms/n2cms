using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence
{
	public abstract class LinqQueryProvider
	{
		public abstract IQueryable<T> Query<T>();
	}
}
