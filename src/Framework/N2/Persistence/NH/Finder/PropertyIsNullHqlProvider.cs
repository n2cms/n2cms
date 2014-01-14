using System.Text;
using NHibernate;

namespace N2.Persistence.NH.Finder
{
    /// <summary>
    /// Represents a property criteria to select values in a certain range.
    /// </summary>
    /// <typeparam name="T">The type of the property value.</typeparam>
    public class PropertyIsNullHqlProvider<T> : IHqlProvider
    {
        private Operator op;
        private string name;
        private bool negate;
        public PropertyIsNullHqlProvider(Operator op, string name, bool negate)
        {
            this.op = op;
            this.name = name;
            this.negate = negate;
        }

        public void AppendHql(StringBuilder from, StringBuilder where, int index)
        {
            where.AppendFormat(" {0} ci.{1} {2}",
                GetOperator(),
                name, GetComparison());
        }
        private string GetComparison()
        {
            return "is " + (negate ? "not " : "") + "null";
        }
        public void SetParameters(IQuery query, int index)
        {
        }

        protected virtual string GetOperator()
        {
            if (op == Operator.None)
                return string.Empty;
            else
                return op.ToString();
        }
    }
}
