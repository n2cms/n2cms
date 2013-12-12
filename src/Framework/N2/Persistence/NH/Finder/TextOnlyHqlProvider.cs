using System.Text;

namespace N2.Persistence.NH.Finder
{
    /// <summary>
    /// Appends custom strings to a hql query.
    /// </summary>
    public class TextOnlyHqlProvider : IHqlProvider
    {
        string from;
        string where;
        Operator op;

        public TextOnlyHqlProvider(string from, string where)
        {
            this.from = from;
            this.where = where;
        }

        public TextOnlyHqlProvider(string from, Operator op, string where)
            : this(from, where)
        {
            this.op = op;
        }
        #region IHqlProvider Members

        public void AppendHql(StringBuilder from, StringBuilder where, int index)
        {
            from.Append(this.from);
            where.AppendFormat(" {0} ", GetOperator());
            where.Append(this.where);
        }

        public void SetParameters(NHibernate.IQuery query, int index)
        {
            // do nothing
        }

        #endregion

        protected virtual string GetOperator()
        {
            if (op == Operator.None)
                return string.Empty;
            else
                return op.ToString();
        }
    }
}
