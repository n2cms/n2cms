using System;
using System.Linq;
using N2.Persistence.Finder;

namespace N2.Persistence.NH.Finder
{
    /// <summary>
    /// The criteria building block of a query. Compares a property to value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PropertyCriteria<T> : IComparisonCriteria<T>
    {
        private string name;
        private Operator op;
        private QueryBuilder query;

        public PropertyCriteria(QueryBuilder query, string name)
        {
            op = query.CurrentOperator;
            this.query = query;
            this.name = name;
        }

        #region IEqualityCriteria<T> Members

        public IQueryAction Eq(T value)
        {
            query.Criterias.Add(new PropertyHqlProvider<T>(op, name, Comparison.Equal, value.EnsureMasterVersion()));
            return query;
        }

        public IQueryAction NotEq(T value)
        {
            query.Criterias.Add(new PropertyHqlProvider<T>(op, name, Comparison.NotEqual, value.EnsureMasterVersion()));
            return query;
        }

        public IQueryAction In(params T[] anyOf)
        {
            if (typeof (T).IsAssignableFrom(typeof (ContentItem)))
            {
                query.Criterias.Add(new PropertyInHqlProvider<int>(op, name, anyOf.OfType<ContentItem>().Select(t => t.VersionOf.ID ?? t.ID).ToArray()));
                return query;
            }
            throw new NotImplementedException();
        }

        #endregion

        #region ICriteria<T> Members

        public IQueryAction Gt(T value)
        {
            query.Criterias.Add(new PropertyHqlProvider<T>(op, name, Comparison.GreaterThan, value));
            return query;
        }

        public IQueryAction Ge(T value)
        {
            query.Criterias.Add(new PropertyHqlProvider<T>(op, name, Comparison.GreaterOrEqual, value));
            return query;
        }

        public IQueryAction Lt(T value)
        {
            query.Criterias.Add(new PropertyHqlProvider<T>(op, name, Comparison.LessThan, value));
            return query;
        }

        public IQueryAction Le(T value)
        {
            query.Criterias.Add(new PropertyHqlProvider<T>(op, name, Comparison.LessOrEqual, value));
            return query;
        }

        public IQueryAction Between(T lowerBound, T upperBound)
        {
            query.Criterias.Add(new PropertyBetweenHqlProvider<T>(op, name, lowerBound, upperBound));
            return query;
        }

        public IQueryAction IsNull(bool isNull)
        {
            query.Criterias.Add(new PropertyIsNullHqlProvider<T>(op, name, !isNull));
            return query;
        }

        #endregion
    }
}
