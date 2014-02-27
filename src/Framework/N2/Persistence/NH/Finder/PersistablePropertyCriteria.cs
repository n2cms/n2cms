using System.Linq;
using N2.Persistence.Finder;

namespace N2.Persistence.NH.Finder
{
    public class PersistablePropertyCriteria : IPropertyCriteria
    {
        private readonly string name;
        private readonly Operator op;
        private readonly QueryBuilder query;

        public PersistablePropertyCriteria(QueryBuilder query, string name)
        {
            this.op = query.CurrentOperator;
            this.query = query;
            this.name = name;
        }

        #region IPropertyCriteria Members

        public IQueryAction Eq<T>(T value)
        {
            query.Criterias.Add(new PropertyHqlProvider<T>(op, name, Comparison.Equal, value));
            return query;
        }

        public IQueryAction NotEq<T>(T value)
        {
            query.Criterias.Add(new PropertyHqlProvider<T>(op, name, Comparison.NotEqual, value));
            return query;
        }

        public IQueryAction Gt<T>(T value)
        {
            query.Criterias.Add(new PropertyHqlProvider<T>(op, name, Comparison.GreaterThan, value));
            return query;
        }

        public IQueryAction Ge<T>(T value)
        {
            query.Criterias.Add(new PropertyHqlProvider<T>(op, name, Comparison.GreaterOrEqual, value));
            return query;
        }

        public IQueryAction Lt<T>(T value)
        {
            query.Criterias.Add(new PropertyHqlProvider<T>(op, name, Comparison.LessThan, value));
            return query;
        }

        public IQueryAction Le<T>(T value)
        {
            query.Criterias.Add(new PropertyHqlProvider<T>(op, name, Comparison.LessOrEqual, value));
            return query;
        }

        public IQueryAction Between<T>(T lowerBound, T upperBound)
        {
            query.Criterias.Add(new PropertyBetweenHqlProvider<T>(op, name, lowerBound, upperBound));
            return query;
        }

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

        public IQueryAction In<T>(params T[] anyOf)
        {
            if (typeof(T).IsAssignableFrom(typeof(ContentItem)))
            {
                query.Criterias.Add(new PropertyInHqlProvider<int>(op, name, anyOf.Select(t => (t as ContentItem).ID).ToArray()));
                return query;
            }

            query.Criterias.Add(new PropertyInHqlProvider<T>(op, name, anyOf));
            return query;
        }

        public IQueryAction Null(bool isNull)
        {
            query.Criterias.Add(new PropertyNullHqlProvider<bool>(op, name, isNull));
            return query;
        }

        #endregion
    }
}
