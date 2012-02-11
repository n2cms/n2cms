using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence
{
	public static class PersistenceExtensions
	{
		public static Parameter Detail(this Parameter parameter, bool isDetail = true)
		{
			parameter.IsDetail = isDetail;
			return parameter;
		}
	}
}
