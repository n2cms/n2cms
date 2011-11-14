using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.Search
{
	/// <summary>
	/// The results of a search query.
	/// </summary>
	public class Result : IEnumerable<ContentItem>
	{
		/// <summary>Search hits.</summary>
		public IEnumerable<Hit> Hits { get; set; }

		/// <summary>Number of items in the result set.</summary>
		public int Count { get; set; }

		/// <summary>Total number of items given the search expression.</summary>
		public int Total { get; set; }

		#region IEnumerable<Hit> Members

		IEnumerator<ContentItem> IEnumerable<ContentItem>.GetEnumerator()
		{
			return Hits.Select(h => h.Content).GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return Hits.Select(h => h.Content).GetEnumerator();
		}

		#endregion

        static Result()
        {
            Empty = new Result() { Count = 0, Total = 0, Hits = new Hit[0] };
        }

        /// <summary>No results.</summary>
        public static Result Empty { get; private set; }
    }
}
