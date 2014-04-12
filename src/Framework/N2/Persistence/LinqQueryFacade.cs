using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence
{
	public abstract class LinqQueryFacade
	{
		public abstract IQueryable<T> Query<T>();
	}
}
