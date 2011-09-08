using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;
using N2.Persistence.Finder;

namespace N2.Persistence.Search
{
	/// <summary>
	/// Searches for text using the finder API which results in LIKE database queries.
	/// </summary>
	[Service(typeof(ITextSearcher))]
	public class FindingTextSearcher : ITextSearcher
	{
		IItemFinder finder;

		public FindingTextSearcher(IItemFinder finder)
		{
			this.finder = finder;
		}

		#region ITextSearcher Members

		/// <summary>Searches for items below an ancestor that matches the given query.</summary>
		/// <param name="ancestor">The ancestor below which the results should be found.</param>
		/// <param name="query">The query text.</param>
		/// <param name="skip">A number of items to skip.</param>
		/// <param name="take">A number of items to take.</param>
		/// <returns>An enumeration of items matching the search query.</returns>
		public IEnumerable<ContentItem> Search(ContentItem ancestor, string query, int skip, int take, bool? onlyPages, Type[] types, out int totalRecords)
		{
			var q = finder.Where.AncestralTrail.Like(Utility.GetTrail(ancestor) + "%")
				.And.Title.IsNull(false);

			var words = query.Split(' ').Where(w => w.Length > 0).Select(w => "%" + w + "%");
			foreach (var word in words)
			{
				q = q.And.OpenBracket()
					.Title.Like(word)
					.Or.Detail().Like(word)
				.CloseBracket();
			}
			if (onlyPages.HasValue)
				q = q.And.OpenBracket()
					.ZoneName.IsNull(onlyPages.Value)
					.Or.ZoneName.Eq("")
				.CloseBracket();
			if (types != null)
				q = q.And.Type.In(types);

			totalRecords = q.Count();
			return q.FirstResult(skip).MaxResults(take).Select();
		}

		public Result Search(Query query)
		{
			var result = new Result();
			int total;
			result.Hits = Search(query.Ancestor, query.Text, query.SkipHits, query.TakeHits, query.OnlyPages, query.Types, out total)
				.Select(i => new Hit { Content = i, Score = (i.Title != null && i.Title.ToLower().Contains(query.Text.ToLower())) ? 1 : .5 });
			result.Total = total;
			return result;
		}

		#endregion
	}
}
