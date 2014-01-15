using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.NH.Finder
{
    public class DetailNullHqlProvider<T> : AbstractHqlProvider<T>
    {
        bool isNull;

        public DetailNullHqlProvider(Operator op, string name, bool isNull)
            : base(op, name, isNull ? Comparison.Null : Comparison.NotNull, default(T))
        {
            this.isNull = isNull;
        }

        public override void AppendHql(StringBuilder from, StringBuilder where, int index)
        {
            string format = isNull 
                ? (Name != null
                    ? " {0} ci not in (select cd.EnclosingItem from ContentDetail cd where cd.{1} is not null AND cd.Name = :{2})"
                    : " {0} ci not in (select cd.EnclosingItem from ContentDetail cd where cd.{1} is not null)")
                : (Name != null
                    ? " {0} ci in (select cd.EnclosingItem from ContentDetail cd where cd.{1} is not null AND cd.Name = :{2})"
                    : " {0} ci in (select cd.EnclosingItem from ContentDetail cd where cd.{1} is not null)");

            where.AppendFormat(format,
                GetOperator(),
                GetDetailValueName(),
                GetNameParameterName(index));
        }

        public override void SetParameters(NHibernate.IQuery query, int index)
        {
            if (this.Name != null)
                query.SetParameter(GetNameParameterName(index), Name);
        }

        protected virtual string GetDetailValueName()
        {
            return Details.ContentDetail.GetAssociatedPropertyName<T>();
        }
    }
}
