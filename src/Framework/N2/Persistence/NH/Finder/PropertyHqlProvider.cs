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
            if (Value != null)
                where.AppendFormat(" {0} ci.{1} {2} :{3}",
                    GetOperator(),
                    Name,
                    GetComparison(),
                    GetValueParameterName(index));
            else
                where.AppendFormat(" {0} ci.{1} is null",
                    GetOperator(),
                    Name,
                    GetComparison(),
                    GetValueParameterName(index));
        }

        public override void SetParameters(NHibernate.IQuery query, int index)
        {
            if(Value != null)
                base.SetParameters(query, index);
        }
    }
}
