using N2.Details;
using System.Text;

namespace N2.Persistence.NH.Finder
{
    /// <summary>
    /// Provides hql for between queries on details.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DetailBetweenHqlProvider<T> : IHqlProvider
    {
        private Operator op;
        private string name;
        private T lowerBound;
        private T upperBound;

        public DetailBetweenHqlProvider(Operator op, string name, T lowerBound, T upperBound)
        {
            this.op = op;
            this.name = name;
            this.lowerBound = lowerBound;
            this.upperBound = upperBound;
        }

        public string GetNameParameterName(int index)
        {
            return "n" + index;
        }

        protected virtual string GetDetailValueName()
        {
            return Details.ContentDetail.GetAssociatedPropertyName(lowerBound);
        }

        protected virtual string GetOperator()
        {
            if (op == Operator.None)
                return string.Empty;
            else
                return op.ToString();
        }

        #region IHqlProvider Members

        public void AppendHql(StringBuilder from, StringBuilder where, int index)
        {
            where.AppendFormat(GetHqlFormat(),
                GetOperator(),
                index,
                GetDetailValueName(),
                "lb" + index,
                "ub" + index,
                GetNameParameterName(index));
        }

        private string GetHqlFormat()
        {
            if(this.name != null)
                return " {0} ci in (select cd.EnclosingItem from ContentDetail cd where cd.{2} between :{3} and :{4} and cd.Name = :{5})";
            else
                return " {0} ci in (select cd.EnclosingItem from ContentDetail cd where cd.{2} between :{3} and :{4})";
        }

        public void SetParameters(NHibernate.IQuery query, int index)
        {
            if(name != null)
                query.SetParameter(GetNameParameterName(index), name);
            query.SetParameter("lb" + index, ContentDetail.ExtractQueryValue(lowerBound));
            query.SetParameter("ub" + index, ContentDetail.ExtractQueryValue(upperBound));
        }

        #endregion
    }
}
