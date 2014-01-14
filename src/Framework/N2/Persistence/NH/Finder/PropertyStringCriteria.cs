using N2.Persistence.Finder;

namespace N2.Persistence.NH.Finder
{
    public class PropertyStringCriteria : IStringCriteria
    {
        private readonly string name;
        private readonly Operator op;
        private readonly QueryBuilder query;

        public PropertyStringCriteria(QueryBuilder query, string name)
        {
            this.op = query.CurrentOperator;
            this.query = query;
            this.name = name;
        }

        #region ICriteria<string> Members

        public IQueryAction IsNull(bool isNull)
        {
            query.Criterias.Add(new PropertyIsNullHqlProvider<string>(op, name, !isNull));

            return query;
        }

        #endregion

        #region IStringCriteria Members

        public IQueryAction Like(string value)
        {
            query.Criterias.Add(new PropertyHqlProvider<string>(op, name, Comparison.Like, value));
            return query;
        }

        public IQueryAction NotLike(string value)
        {
            query.Criterias.Add(new PropertyHqlProvider<string>(op, name, Comparison.NotLike, value));
            return query;
        }

        #endregion

        #region IEqualityCriteria<string> Members

        public IQueryAction Eq(string value)
        {
            query.Criterias.Add(new PropertyHqlProvider<string>(op, name, Comparison.Equal, value));
            return query;
        }

        public IQueryAction NotEq(string value)
        {
            query.Criterias.Add(new PropertyHqlProvider<string>(op, name, Comparison.NotEqual, value));
            return query;
        }

        public IQueryAction In(params string[] values)
        {
            query.Criterias.Add(new PropertyInHqlProvider<string>(op, name, values));
            return query;
        }

        #endregion
    }
}
