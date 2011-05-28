using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;
using Lucene.Net.Search;
using Lucene.Net.QueryParsers;
using System.Diagnostics;

namespace N2.Persistence.Search
{
	[Service(typeof(ITextSearcher), Replaces = typeof(FindingTextSearcher))]
	public class LuceneSearcher : ITextSearcher
	{
		LuceneAccesor accessor;
		IPersister persister;

		public LuceneSearcher(LuceneAccesor accessor, IPersister persister)
		{
			this.accessor = accessor;
			this.persister = persister;
		}

		#region ITextSearcher Members

		public SearchResult Search(SearchQuery query)
		{
			var s = accessor.GetSearcher();
			try
			{
				var q = CreateQuery(query);
				var hits = s.Search(q, query.Skip + query.Take);

				var result = new SearchResult();
				result.Total = hits.totalHits;
				result.Hits = hits.scoreDocs.Skip(query.Skip).Take(query.Take).Select(hit =>
				{
					var doc = s.Doc(hit.doc);
					int id = int.Parse(doc.Get("ID"));
					ContentItem item = persister.Get(id);
					return new SearchHit { Content = item, Score = hit.score };
				}).ToList();
				return result;
			}
			finally
			{
				s.Close();
			}
		}

		protected virtual Query CreateQuery(SearchQuery query)
		{
			var q = query.OnlyPages.HasValue && query.OnlyPages.Value
				? string.Format("+(Title:({0})^4 Text:({0}) PartsText:({0}))", query.Text)
				: string.Format("+(Title:({0})^4 Text:({0}))", query.Text);

			if (query.Ancestor != null)
				q += string.Format(" +Trail:{0}*", Utility.GetTrail(query.Ancestor));
			if(query.OnlyPages.HasValue)
				q += string.Format(" +IsPage:{0}", query.OnlyPages.Value.ToString().ToLower());
			if (query.Roles != null)
				q += string.Format(" +Roles:(Everyone {0})", string.Join(" ", query.Roles.ToArray()));

			Trace.WriteLine("CreateQuery: " + q);

			return accessor.GetQueryParser().Parse(q);
		}
		#endregion
	}
}
