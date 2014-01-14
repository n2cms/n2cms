using System.Text;

namespace N2.Persistence.NH.Finder
{
    /// <summary>
    /// Creates the hql query for finding versions of an item.
    /// </summary>
    public class VersionOfHqlProvider : IHqlProvider
    {
        Operator op;
        int? itemID;

        public VersionOfHqlProvider(Operator op, ContentItem item)
        {
            this.op=op;
            if (item != null)
                this.itemID = item.ID;
        }

        #region IHqlProvider Members

        public void AppendHql(StringBuilder from, StringBuilder where, int index)
        {
            if (itemID.HasValue)
                where.AppendFormat(" {0} VersionOf.ID = :v{1}",
                    GetOperator(),
                    index);
            else
                where.AppendFormat(" {0} VersionOf.ID IS NULL",
                    GetOperator());
        }

        public void SetParameters(NHibernate.IQuery query, int index)
        {
            if(itemID.HasValue)
                query.SetParameter("v" + index, this.itemID.Value);
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
