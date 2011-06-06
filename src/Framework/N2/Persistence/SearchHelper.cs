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
		IEngine engine;

		public SearchHelper(IEngine engine)
		{
			this.engine = engine;
		}

		public IQueryable<ContentItem> Items
		{
			get { return Query<ContentItem>().Where(i => i.VersionOf == null); }
		}

		public IItemFinder Find
		{
			get { return engine.Resolve<IItemFinder>(); }
		}

		public ITextSearcher Text
		{
			get { return engine.Resolve<ITextSearcher>(); }
		}

		public virtual IQueryable<T> Query<T>()
		{
			return engine.Query<T>();
		}
	}
}
