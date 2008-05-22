using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;

namespace N2.Persistence.NH.Finder
{
	public class PropertyHqlProvider : AbstractHqlProvider<object>
	{
		public PropertyHqlProvider(Operator op, string name, Comparison comparison, object value)
			: base(op, name, comparison, value)
		{
		}

		public override void AppendHql(StringBuilder from, StringBuilder where, int index)
		{
			where.AppendFormat(" {0} ci.{1} {2} :{3}",
				GetOperator(),
				Name,
				GetComparison(),
				GetValueParameterName(index));
		}
	}
}