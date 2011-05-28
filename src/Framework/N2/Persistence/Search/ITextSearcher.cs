using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Persistence.Search;

namespace N2.Persistence.Search
{
	/// <summary>
	/// Searches for text stored in the system.
	/// </summary>
	public interface ITextSearcher
	{
		/// <summary>Searches for items below an ancestor that matches the given query. The results are not checked for permissions.</summary>
		/// <param name="query">An object containing the text to search for.</param>
		/// <returns>A result object containing an enumeration of items matching the search query.</returns>
		SearchResult Search(SearchQuery query);
	}
}
