using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Linq;

namespace N2.Linq
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> Cached<T>(IQueryable<T> query, NHibernate.CacheMode mode = NHibernate.CacheMode.Normal, string cacheRegion = null)
        {
            query = query.Cacheable();
            if (mode != NHibernate.CacheMode.Normal)
                query = query.CacheMode(mode);
            if (cacheRegion != null)
                query = query.CacheRegion(cacheRegion);
            return query;
        }

        public static IQueryable<TSource> WherePublished<TSource>(this IQueryable<TSource> source) where TSource : ContentItem
        {
            return source.Where(ci => ci.State == ContentState.Published);
        }

        public static IQueryable<TSource> WhereAncestorOf<TSource>(this IQueryable<TSource> source, ContentItem descendant) where TSource : ContentItem
        {
            var ancestorIDs = descendant.AncestralTrail.Split('/').Where(id => !string.IsNullOrEmpty(id)).Select(id => int.Parse(id)).ToArray();
            return source.Where(ci => ancestorIDs.Contains(ci.ID));
        }

        public static IQueryable<TSource> WhereDescendantOf<TSource>(this IQueryable<TSource> source, ContentItem ancestor) where TSource : ContentItem
        {
            var trail = ancestor.GetTrail();
            return source.Where(ci => ci.AncestralTrail.StartsWith(trail));
        }

        public static IQueryable<TSource> WhereDescendantOrSelf<TSource>(this IQueryable<TSource> source, ContentItem ancestor) where TSource : ContentItem
        {
            var trail = ancestor.GetTrail();
            return source.Where(ci => ci.AncestralTrail.StartsWith(trail) || ci == ancestor);
        }

        public static IQueryable<TSource> WherePage<TSource>(this IQueryable<TSource> source, bool isPage = true) where TSource : ContentItem
        {
            if (isPage)
                return source.Where(ci => ci.ZoneName == null);
            else
                return source.Where(ci => ci.ZoneName != null);
        }

        public static IQueryable<TSource> WherePrecedingSiblingOf<TSource>(this IQueryable<TSource> source, ContentItem precedingSibling) where TSource : ContentItem
        {
            return source.Where(ci => ci.SortOrder < precedingSibling.SortOrder && ci.Parent == precedingSibling.Parent);
        }

        public static IQueryable<TSource> WhereSubsequentSiblingOf<TSource>(this IQueryable<TSource> source, ContentItem precedingSibling) where TSource : ContentItem
        {
            return source.Where(ci => ci.SortOrder > precedingSibling.SortOrder && ci.Parent == precedingSibling.Parent);
        }

        static MethodInfo whereMethodInfo = typeof(Queryable).GetMethods().First(m => m.Name == "Where" && m.GetParameters().Length == 2).GetGenericMethodDefinition();
        
        public static IQueryable<TSource> WhereDetail<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate) where TSource : ContentItem
        {
            var whereOfT = whereMethodInfo.MakeGenericMethod(new Type[] { typeof(TSource) });
            var transformedExpression = new QueryTransformer().ToDetailSubselect<TSource>(predicate);
            var whereDetailSubselectExpression = Expression.Call(whereOfT, source.Expression, transformedExpression);
            return source.Provider.CreateQuery<TSource>(whereDetailSubselectExpression);
        }

        public static IQueryable<T> WhereDetailEquals<T, TValue>(this IQueryable<T> query, TValue value) where T : ContentItem
        {
            var queryByName = query.Where(
                StringComparison<T>(null, value as string)
                ?? ContentItemComparison<T>(null, value as ContentItem)
                ?? IntegerComparison<T>(null, value as int?)
                ?? DateTimeComparison<T>(null, value as DateTime?)
                ?? BooleanComparison<T>(null, value as bool?)
                ?? DoubleComparison<T>(null, value as double?)
                ?? UnknownValueType<T>(null, value));

            return queryByName;
        }

        public static IQueryable<T> WhereDetailEquals<T, TValue>(this IQueryable<T> query, string name, TValue value) where T : ContentItem
        {
            var queryByName = query.Where(
                StringComparison<T>(name, value as string)
                ?? ContentItemComparison<T>(name, value as ContentItem)
                ?? IntegerComparison<T>(name, value as int?)
                ?? DateTimeComparison<T>(name, value as DateTime?)
                ?? BooleanComparison<T>(name, value as bool?)
                ?? DoubleComparison<T>(name, value as double?)
                ?? UnknownValueType<T>(name, value));

            return queryByName;
        }

        private static Expression<Func<T, bool>> BooleanComparison<T>(string name, bool? value) where T : ContentItem
        {
            if (value == null)
                return null;

            if (name == null) return (ci) => ci.Details.Any(cd => cd.BoolValue == value.Value);
            return (ci) => ci.Details.Any(cd => cd.Name == name && cd.BoolValue == value.Value);
        }

        private static Expression<Func<T, bool>> IntegerComparison<T>(string name, int? value) where T : ContentItem
        {
            if (value == null)
                return null;

            if (name == null) return (ci) => ci.Details.Any(cd => cd.Name == name && cd.IntValue == value.Value);
            return (ci) => ci.Details.Any(cd => cd.Name == name && cd.IntValue == value.Value);
        }

        private static Expression<Func<T, bool>> DoubleComparison<T>(string name, double? value) where T : ContentItem
        {
            if (value == null)
                return null;

            if (name == null) return (ci) => ci.Details.Any(cd => cd.DoubleValue == value.Value);
            return (ci) => ci.Details.Any(cd => cd.Name == name && cd.DoubleValue == value.Value);
        }

        private static Expression<Func<T, bool>> DateTimeComparison<T>(string name, DateTime? value) where T : ContentItem
        {
            if (value == null)
                return null;

            if (name == null) return (ci) => ci.Details.Any(cd => cd.DateTimeValue == value.Value);
            return (ci) => ci.Details.Any(cd => cd.Name == name && cd.DateTimeValue == value.Value);
        }

        private static Expression<Func<T, bool>> StringComparison<T>(string name, string value) where T : ContentItem
        {
            if (value == null)
                return null;

            if (name == null) return (ci) => ci.Details.Any(cd => cd.StringValue == value);
            return (ci) => ci.Details.Any(cd => cd.Name == name && cd.StringValue == value);
        }

        private static Expression<Func<T, bool>> ContentItemComparison<T>(string name, ContentItem value) where T : ContentItem
        {
            if (value == null)
                return null;

            if (name == null) return (ci) => ci.Details.Any(cd => cd.LinkedItem == value);

            return (ci) => ci.Details.Any(cd => cd.Name == name && cd.LinkedItem == value);
        }

        private static Expression<Func<T, bool>> UnknownValueType<T>(string name, object value) where T : ContentItem
        {
            throw new NotSupportedException(value + " is not supported.");
        }
    }
}
