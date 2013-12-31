using N2.Persistence.Finder;

namespace N2.Persistence.NH.Finder
{
    public class DetailCriteria : IDetailCriteria
    {
        private readonly string name;
        private readonly Operator op;
        private readonly QueryBuilder query;

        public DetailCriteria(QueryBuilder query, string name)
        {
            op = query.CurrentOperator;
            this.query = query;
            this.name = name;
        }

        #region IDetailCriteria Members

        public IQueryAction Eq<T>(T value)
        {
            query.Criterias.Add(new DetailHqlProvider<T>(op, name, Comparison.Equal, value));
            return query;
        }

        public IQueryAction NotEq<T>(T value)
        {
            query.Criterias.Add(new DetailHqlProvider<T>(op, name, Comparison.NotEqual, value));
            return query;
        }

        public IQueryAction Gt<T>(T value)
        {
            query.Criterias.Add(new DetailHqlProvider<T>(op, name, Comparison.GreaterThan, value));
            return query;
        }

        public IQueryAction Ge<T>(T value)
        {
            query.Criterias.Add(new DetailHqlProvider<T>(op, name, Comparison.GreaterOrEqual, value));
            return query;
        }

        public IQueryAction Lt<T>(T value)
        {
            query.Criterias.Add(new DetailHqlProvider<T>(op, name, Comparison.LessThan, value));
            return query;
        }

        public IQueryAction Le<T>(T value)
        {
            query.Criterias.Add(new DetailHqlProvider<T>(op, name, Comparison.LessOrEqual, value));
            return query;
        }

        public IQueryAction Between<T>(T lowerBound, T upperBound)
        {
            query.Criterias.Add(new DetailBetweenHqlProvider<T>(op, name, lowerBound, upperBound));
            return query;
        }

        public IQueryAction Like(string value)
        {
            query.Criterias.Add(new DetailHqlProvider<string>(op, name, Comparison.Like, value ?? ""));
            return query;
        }

        public IQueryAction NotLike(string value)
        {
            query.Criterias.Add(new DetailHqlProvider<string>(op, name, Comparison.NotLike, value ?? ""));
            return query;
        }

        public IQueryAction In<T>(params T[] values)
        {
            query.Criterias.Add(new DetailInHqlProvider<T>(op, name, values));
            return query;
        }

        public IQueryAction Null<T>(bool isNull)
        {
            query.Criterias.Add(new DetailNullHqlProvider<T>(op, name, isNull));
            return query;
        }

        #endregion
    }
}
