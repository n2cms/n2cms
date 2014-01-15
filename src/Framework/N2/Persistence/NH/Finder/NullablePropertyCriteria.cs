using N2.Persistence.Finder;

namespace N2.Persistence.NH.Finder
{
    /// <summary>
    /// The criteria building block of a query. Compares a property to value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NullablePropertyCriteria<T> : PropertyCriteria<T>, INullableComparisonCriteria<T>
    {
        private string name;
        private Operator op;
        private QueryBuilder query;

        public NullablePropertyCriteria(QueryBuilder query, string name)
            : base(query,name)
        {
            op = query.CurrentOperator;
            this.query = query;
            this.name = name;
        }

        public IQueryAction IsNull()
        {
            query.Criterias.Add(new PropertyIsNullHqlProvider<T>(op, name, false));
            return query;
        }

        public IQueryAction IsNotNull()
        {
            query.Criterias.Add(new PropertyIsNullHqlProvider<T>(op, name, true));
            return query;
        }
    }
}
