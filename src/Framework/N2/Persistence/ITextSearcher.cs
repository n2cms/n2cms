using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence
{
	/// <summary>
	/// Searches for text stored in the system.
	/// </summary>
	public interface ITextSearcher
	{
		/// <summary>Searches for items below an ancestor that matches the given query. The results are not checked for permissions.</summary>
		/// <param name="ancestor">The ancestor below which the results should be found.</param>
		/// <param name="query">The query text.</param>
		/// <param name="skip">A number of items to skip.</param>
		/// <param name="take">A number of items to take.</param>
		/// <returns>An enumeration of items matching the search query.</returns>
		IEnumerable<ContentItem> Search(ContentItem ancestor, string query, int skip, int take, out int totalRecords);
	}
}
