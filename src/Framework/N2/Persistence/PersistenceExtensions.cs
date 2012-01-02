using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence
{
	public static class PersistenceExtensions
	{
		public static IEnumerable<T> Find<T>(this IRepository<T> repository, ParameterCollection parameters)
		{
			return repository.Find((IParameter)parameters);
		}

		public static ParameterCollection ToCollection(this IParameter parameter)
		{
			return parameter as ParameterCollection ?? new ParameterCollection(parameter);
		}
	}
}
