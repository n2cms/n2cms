using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Persistence.NH.Finder
{
	public class DetailInHqlProvider<T> : AbstractInHqlProvider<T>
	{
		public DetailInHqlProvider(Operator op, string name, T[] values) 
			: base(op, name, values)
		{
		}

		public override void AppendHql(StringBuilder from, StringBuilder where, int index)
		{
			string format = Name != null
			                	? " {0} ci in (select cd.EnclosingItem from ContentDetail cd where cd.{1} in ({2}) AND cd.Name = :{3})"
			                	: " {0} ci in (select cd.EnclosingItem from ContentDetail cd where cd.{1} in ({2}))";

			string parameters = GetParameters(index);
			where.AppendFormat(format, 
				GetOperator(),
				GetDetailValueName(),
				parameters,
				Name);
		}

		protected virtual string GetDetailValueName()
		{
			return Details.ContentDetail.GetAssociatedPropertyName(Values[0]);
		}
	}
}
