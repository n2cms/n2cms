using N2.Details;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.NH
{
    internal static class NHExtensions
    {
        public static ICriteria CreateCriteria<TEntity>(this ISession session, IParameter parameter)
        {
            var expr = parameter.CreateCriterion();
            var crit = RepositoryHelper<TEntity>.CreateCriteriaFromArray(session, new[] { expr });

            var pc = parameter as ParameterCollection;
            if (pc != null)
            {
                if (pc.Order != null)
                {
                    crit.AddOrder(new NHibernate.Criterion.Order(pc.Order.Property, !pc.Order.Descending));
                }
                if (pc.Range != null)
                {
                    if (pc.Range.Skip > 0)
                        crit.SetFirstResult(pc.Range.Skip);
                    if (pc.Range.Take > 0)
                        crit.SetMaxResults(pc.Range.Take);
                }
            }
            return crit;
        }


        public static NHibernate.Criterion.ICriterion CreateCriterion(this IParameter parameter)
        {
            if (parameter is Parameter)
            {
                var p = parameter as Parameter;
                if (p != null && p.IsDetail)
                    return CreateDetailExpression(p);

                return CreateExpression(p.Name, p.Value, p.Comparison, false);
            }
            else if (parameter is ParameterCollection)
            {
                var pc = parameter as ParameterCollection;
                var x = pc.Operator == Operator.And
                    ? (Junction)Expression.Conjunction()
                    : (Junction)Expression.Disjunction();
                foreach (var p in pc)
                    x.Add(CreateCriterion(p));

                return x;
            }
            throw new NotImplementedException();
        }

        private static ICriterion CreateDetailExpression(Parameter p)
        {
            if (p.Comparison == Comparison.NotNull)
                return Subqueries.PropertyIn("ID",
                    DetachedCriteria.For<ContentDetail>()
                        .SetProjection(Projections.Property("EnclosingItem.ID"))
                        .Add(Expression.Eq("Name", p.Name)));
            if (p.Comparison == Comparison.Null)
                return Subqueries.PropertyNotIn("ID",
                    DetachedCriteria.For<ContentDetail>()
                        .SetProjection(Projections.Property("EnclosingItem.ID"))
                        .Add(Expression.Eq("Name", p.Name)));

            string propertyName = p.Comparison.HasFlag(Comparison.In)
                ? ContentDetail.GetAssociatedEnumerablePropertyName(p.Value as IEnumerable)
                : ContentDetail.GetAssociatedPropertyName(p.Value);

            var subselect = DetachedCriteria.For<ContentDetail>()
                .SetProjection(Projections.Property("EnclosingItem.ID"))
                .Add(CreateExpression(propertyName, ContentDetail.ExtractQueryValue(p.Value), p.Comparison, true));
            
            if (p.Name != null)
                subselect = subselect.Add(Expression.Eq("Name", p.Name));

            return Subqueries.PropertyIn("ID", subselect);
        }

        private static ICriterion CreateExpression(string propertyName, object value, Comparison comparisonType, bool isDetail)
        {
            if (value == null)
                return Expression.IsNull(propertyName);

            if (propertyName == "LinkedItem")
            {
                propertyName = "LinkedItem.ID";
                value = Utility.GetProperty(value, "ID");
            }

            switch (comparisonType)
            {
                case Comparison.Equal:
                    return Expression.Eq(propertyName, value);
                case Comparison.GreaterOrEqual:
                    return Expression.Ge(propertyName, value);
                case Comparison.GreaterThan:
                    return Expression.Gt(propertyName, value);
                case Comparison.LessOrEqual:
                    return Expression.Le(propertyName, value);
                case Comparison.LessThan:
                    return Expression.Lt(propertyName, value);
                case Comparison.Like:
                    return Expression.Like(propertyName, value);
                case Comparison.NotEqual:
                    return Expression.Not(Expression.Eq(propertyName, value));
                case Comparison.NotLike:
                    return Expression.Not(Expression.Like(propertyName, value));
                case Comparison.NotNull:
                    return Expression.IsNotNull(propertyName);
                case Comparison.Null:
                    return Expression.IsNull(propertyName);
                case Comparison.In:
                    return Expression.In(propertyName, ((IEnumerable)value).OfType<object>().ToArray());
                case Comparison.NotIn:
                    return Expression.Not(Expression.In(propertyName, ((IEnumerable)value).OfType<object>().ToArray()));
            }

            if (value is string)
                return Expression.Like(propertyName, value);

            return Expression.Eq(propertyName, value);
        }

        public static IEnumerable<IDictionary<string, object>> SelectProperties(this ICriteria crit, string[] properties)
        {
            var projection = Projections.ProjectionList();
            foreach (var property in properties)
                projection.Add(Projections.Property(property));

            if (properties.Length == 1)
                return crit.SetProjection(projection)
                    .List<object>()
                    .Select(col => new Dictionary<string, object>() { { properties[0], col } });

            return crit.SetProjection(projection)
                .List<System.Collections.IList>()
                .Select(l =>
                {
                    var row = new Dictionary<string, object>();
                    for (int i = 0; i < properties.Length; i++)
                        row[properties[i]] = l[i];
                    return row;
                });
        }
    }
}
