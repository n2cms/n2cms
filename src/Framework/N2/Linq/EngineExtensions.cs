using System.Linq;
using N2.Details;
using N2.Engine;
using N2.Persistence.Finder;
using N2.Persistence.NH;
using NHibernate.Linq;
using System;
using N2.Persistence;

namespace N2.Linq
{
    public static class EngineExtensions
    {
        /// <summary>Creates a query for elements in the N2 database.</summary>
        /// <typeparam name="T">The type of item to query.</typeparam>
        /// <param name="engine">The engine onto which the extension method is attached.</param>
        /// <returns>A query that can be extended.</returns>
        public static IQueryable<T> Query<T>(this IEngine engine)
        {
            var q = engine.Resolve<LinqQueryFacade>().Query<T>();
            return q;
        }

        /// <summary>Creates a query for published content items.</summary>
        /// <typeparam name="T">The type of content item to query for.</typeparam>
        /// <param name="engine">The engine onto which the extension method is attached.</param>
        /// <returns>A query that can be extended.</returns>
        public static IQueryable<T> QueryItems<T>(this IEngine engine) where T : ContentItem
        {
            return engine.QueryItems<T>(VersionOption.Exclude);
        }

        /// <summary>Create a query for content items with the option of including versions.</summary>
        /// <typeparam name="T">The type of content item to query for.</typeparam>
        /// <param name="engine">The engine onto which the extension method is attached.</param>
        /// <param name="versions">Wheter to include or exclude versions.</param>
        /// <returns>A query that can be extended.</returns>
        public static IQueryable<T> QueryItems<T>(this IEngine engine, VersionOption versions) where T : ContentItem
        {
            var q = engine.Query<T>();
            if (versions == N2.Persistence.Finder.VersionOption.Exclude)
                q = q.Where(i => i.VersionOf.ID == null);
            return q;
        }

        /// <summary>Creates a query for published content items.</summary>
        /// <param name="engine">The engine onto which the extension method is attached.</param>
        /// <returns>A query that can be extended.</returns>
        public static IQueryable<ContentItem> QueryItems(this IEngine engine)
        {
            return engine.QueryItems<ContentItem>(VersionOption.Exclude);
        }

        /// <summary>Creates a query for content items with the option of including versions.</summary>
        /// <param name="versions">Wheter to include or exclude versions.</param>
        /// <param name="engine">The engine onto which the extension method is attached.</param>
        /// <returns>A query that can be extended.</returns>
        public static IQueryable<ContentItem> QueryItems(this IEngine engine, VersionOption versions)
        {
            return engine.QueryItems<ContentItem>(versions);
        }

        /// <summary>Creates a query for details in the N2 database.</summary>
        /// <param name="engine">The engine onto which the extension method is attached.</param>
        /// <returns>A query that can be extended.</returns>
        public static IQueryable<ContentDetail> QueryDetails(this IEngine engine)
        {
            return engine.Query<ContentDetail>();
        }

        /// <summary>Creates a query for detail collections.</summary>
        /// <param name="engine">The engine onto which the extension method is attached.</param>
        /// <returns>A query that can be extended.</returns>
        public static IQueryable<DetailCollection> QueryDetailCollections(this IEngine engine)
        {
            return engine.Query<DetailCollection>();
        }
    }
}
