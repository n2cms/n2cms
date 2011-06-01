using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.Search
{
	/// <summary>
	/// The results of a search query.
	/// </summary>
	public class Result
	{
		/// <summary>Search hits.</summary>
		public IEnumerable<Hit> Hits { get; set; }

		/// <summary>Total number of items given the search expression.</summary>
		public int Total { get; set; }
	}
}
