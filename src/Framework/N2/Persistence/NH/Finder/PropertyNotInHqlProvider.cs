using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.NH.Finder
{
	public class PropertyNotInHqlProvider<T> : AbstractInHqlProvider<T>
	{
		public PropertyNotInHqlProvider(Operator op, string name, T[] values)
			: base(op, name, values)
		{
		}

		public override void AppendHql(StringBuilder from, StringBuilder where, int index)
		{
			string parameters = GetParameters(index);
			where.AppendFormat(" {0} ci.{1} not in ({2})", 
				GetOperator(),
				Name,
				parameters);
		}
	}
}
