using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Persistence.Finder;
using N2.Engine;
using N2.Persistence.Search;
using N2.Linq;

namespace N2.Persistence
{
    /// <summary>
    /// Simplifies access to APIs related to search and querying.
    /// </summary>
    public class SearchHelper
    {
        Func<IEngine> engine;

        public SearchHelper(Func<IEngine> engine)
        {
            this.engine = engine;
        }

        /// <summary>Queries a persisted entity using LINQ.</summary>
        /// <typeparam name="T">The type of entity to query. Only types mapped via NHibernate are queryable.</typeparam>
        /// <returns>A queryable expression.</returns>
        public virtual IQueryable<T> Query<T>()
        {
            return engine().Query<T>();
        }

        /// <summary>Query for content items using LINQ.</summary>
        /// <remarks>Pending or expired pages may be included in the result.</remarks>
        public IQueryable<ContentItem> Items
        {
            get { return Query<ContentItem>().Where(i => i.VersionOf.ID == null); }
        }

        /// <summary>Query for content pages using LINQ.</summary>
        /// <remarks>Pending or expired pages may be included in the result.</remarks>
        public IQueryable<ContentItem> Pages
        {
            get { return Items.WherePage(isPage: true); }
        }

        /// <summary>Query for published content pages using LINQ.</summary>
        public IQueryable<ContentItem> PublishedPages
        {
            get { return Pages.WherePublished(); }
        }

        /// <summary>Query for published content pages below a certain root using LINQ.</summary>
        public IQueryable<ContentItem> PublishedPagesBelow(ContentItem ancestorOrSelf)
        {
            return PublishedPages.WhereDescendantOrSelf(ancestorOrSelf);
        }


        /// <summary>Allows finding items using the Find(Parameter...)</summary>
        public IContentItemRepository Repository
        {
            get { return engine().Persister.Repository; }
        }


        /// <summary>Find items using the finder API.</summary>
        /// <remarks>This API is comparable to using LINQ but not as comperhensive.</remarks>
        public IItemFinder Find
        {
            get { return engine().Resolve<IItemFinder>(); }
        }



        /// <summary>Performs text search for content items.</summary>
        public IContentSearcher Text
        {
            get { return engine().Resolve<IContentSearcher>(); }
        }
    }
}
