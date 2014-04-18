using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.Search
{
    /// <summary>
    /// The results of a search query.
    /// </summary>
    public class Result<T> : IEnumerable<T> where T : class
    {
        /// <summary>Search hits.</summary>
        public IEnumerable<Hit<T>> Hits { get; set; }

		private int? count;
		/// <summary>Number of items in the result set.</summary>
		public int Count
		{
			get { return count ?? (count = ((Hits != null) ? Hits.Count() : 0)) ?? 0; }
			set { count = value; }
		}

        /// <summary>Total number of items given the search expression.</summary>
        public int Total { get; set; }

        #region IEnumerable<Hit> Members

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
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
            Empty = new Result<T>() { Count = 0, Total = 0, Hits = new Hit<T>[0] };
        }

        /// <summary>No results.</summary>
        public static Result<T> Empty { get; private set; }
    }
}
