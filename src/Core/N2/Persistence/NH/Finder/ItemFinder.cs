using System;
using System.Collections.Generic;
using System.Text;
using N2.Definitions;
using N2.Persistence.Finder;

namespace N2.Persistence.NH.Finder
{
	/// <summary>
	/// Provides the query builder.
	/// </summary>
	public class ItemFinder : N2.Persistence.Finder.IItemFinder
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
