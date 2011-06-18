using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.Search
{
	/// <summary>
	/// A single search result.
	/// </summary>
	public class Hit
	{
		/// <summary>
		/// The content item found.
		/// </summary>
		public ContentItem Content { get; set; }

		/// <summary>
		/// The search hit score between 0 and 1.
		/// </summary>
		public double Score { get; set; }
	}
}
