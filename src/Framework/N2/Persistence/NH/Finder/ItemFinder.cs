using N2.Definitions;
using N2.Persistence.Finder;
using N2.Engine;

namespace N2.Persistence.NH.Finder
{
	/// <summary>
	/// Provides the query builder.
	/// </summary>
	[Service(typeof(IItemFinder))]
	public class ItemFinder : IItemFinder
	{
		ISessionProvider sessionProvider;
		private readonly IDefinitionManager definitions;

		public ItemFinder(ISessionProvider sessionProvider, IDefinitionManager definitions)
		{
			this.sessionProvider = sessionProvider;
			this.definitions = definitions;
		}

		/// <summary>Starts the building of a query.</summary>
		public IQueryBuilder Where
		{
			get { return new QueryBuilder(sessionProvider, definitions); }
		}

		/// <summary>Allows selection of all items.</summary>
		public IQueryEnding All
		{
			get { return new QueryBuilder(sessionProvider, definitions); }
		}
	}
}
