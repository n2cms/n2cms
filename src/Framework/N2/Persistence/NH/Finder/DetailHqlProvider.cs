using N2.Details;
using System.Text;

namespace N2.Persistence.NH.Finder
{
    /// <summary>
    /// Provides hql for queries on details.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DetailHqlProvider<T> : AbstractHqlProvider<T>
    {
        public DetailHqlProvider(Operator op, string name, Comparison comparison, T value)
            : base(op, name, comparison, value)
        {
        }

        protected virtual string GetDetailValueName()
        {
            return Details.ContentDetail.GetAssociatedPropertyName(Value);
        }

        protected string GetFormat()
        {
            if (this.Name != null)
                return " {0} ci in (select cd.EnclosingItem from ContentDetail cd where cd.{2} {3} :{4} AND cd.Name = :{5})";
            else
                return " {0} ci in (select cd.EnclosingItem from ContentDetail cd where cd.{2} {3} :{4})";
        }

        public override void AppendHql(StringBuilder from, StringBuilder where, int index)
        {
            where.AppendFormat(GetFormat(),
                GetOperator(),
                index,
                GetDetailValueName(),
                GetComparison(),
                GetValueParameterName(index),
                GetNameParameterName(index));
        }

        public override void SetParameters(NHibernate.IQuery query, int index)
        {
            if (this.Name != null) 
                query.SetParameter(GetNameParameterName(index), Name);
            query.SetParameter(GetValueParameterName(index), ContentDetail.ExtractQueryValue(Value));
        }
    }
}
