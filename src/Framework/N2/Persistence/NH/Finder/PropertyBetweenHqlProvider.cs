using System.Text;
using NHibernate;

namespace N2.Persistence.NH.Finder
{
    /// <summary>
    /// Represents a property criteria to select values in a certain range.
    /// </summary>
    /// <typeparam name="T">The type of the property value.</typeparam>
    public class PropertyBetweenHqlProvider<T> : IHqlProvider
    {
        private Operator op;
        private string name;
        private T lowerBound;
        private T upperBound;

        public PropertyBetweenHqlProvider(Operator op, string name, T lowerBound, T upperBound)
        {
            this.op = op;
            this.name = name;
            this.lowerBound = lowerBound;
            this.upperBound = upperBound;
        }

        #region IHqlProvider Members

        public void AppendHql(StringBuilder from, StringBuilder where, int index)
        {
            where.AppendFormat(" {0} ci.{1} between :{2} and :{3}",
                GetOperator(),
                name,
                "lb" + index,
                "ub" + index);
        }

        public void SetParameters(IQuery query, int index)
        {
            query.SetParameter("lb" + index, lowerBound);
            query.SetParameter("ub" + index, upperBound);
        }

        protected virtual string GetOperator()
        {
            if (op == Operator.None)
                return string.Empty;
            else
                return op.ToString();
        }
        #endregion
    }
}
