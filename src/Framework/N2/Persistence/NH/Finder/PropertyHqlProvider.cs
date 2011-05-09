using System.Text;

namespace N2.Persistence.NH.Finder
{
	public class PropertyHqlProvider<T> : AbstractHqlProvider<T>
	{
		public PropertyHqlProvider(Operator op, string name, Comparison comparison, T value)
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