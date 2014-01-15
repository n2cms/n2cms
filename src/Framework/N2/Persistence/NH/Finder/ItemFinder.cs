using N2.Definitions.Static;
using N2.Engine;
using N2.Persistence.Finder;

namespace N2.Persistence.NH.Finder
{
    /// <summary>
    /// Provides the query builder.
    /// </summary>
    [Service(typeof(IItemFinder))]
    public class ItemFinder : IItemFinder
    {
        ISessionProvider sessionProvider;
        DefinitionMap map;

        public ItemFinder(ISessionProvider sessionProvider, DefinitionMap map)
        {
            this.sessionProvider = sessionProvider;
            this.map = map;
        }

        /// <summary>Starts the building of a query.</summary>
        public IQueryBuilder Where
        {
            get { return new QueryBuilder(sessionProvider, map); }
        }

        /// <summary>Allows selection of all items.</summary>
        public IQueryEnding All
        {
            get { return new QueryBuilder(sessionProvider, map); }
        }
    }
}
