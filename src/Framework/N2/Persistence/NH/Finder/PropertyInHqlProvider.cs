using System.Text;

namespace N2.Persistence.NH.Finder
{
    public class PropertyInHqlProvider<T> : AbstractInHqlProvider<T>
    {
        public PropertyInHqlProvider(Operator op, string name, T[] values)
            : base(op, name, values)
        {
        }

        public override void AppendHql(StringBuilder from, StringBuilder where, int index)
        {
            string parameters = GetParameters(index);
            where.AppendFormat(" {0} ci.{1} in ({2})", 
                GetOperator(),
                Name,
                parameters);
        }
    }
}
