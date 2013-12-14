using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.NH.Finder
{
    public class PropertyNullHqlProvider<T> : AbstractHqlProvider<T>
    {
        bool isNull;

        public PropertyNullHqlProvider(Operator op, string name, bool isNull)
            : base(op, name, isNull ? Comparison.Null : Comparison.NotNull, default(T))
        {
            this.isNull = isNull;
        }

        public override void SetParameters(NHibernate.IQuery query, int index)
        {
        }

        public override void AppendHql(StringBuilder from, StringBuilder where, int index)
        {
            where.AppendFormat(" {0} ci.{1} is " + (isNull ? "null" : "not null"),
                GetOperator(),
                Name);
        }
    }
}
