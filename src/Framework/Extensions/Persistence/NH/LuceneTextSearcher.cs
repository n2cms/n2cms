using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;

namespace N2.Persistence.NH
{
	[Service(typeof(ITextSearcher), Replaces = typeof(FindingTextSearcher))]
	public class LuceneTextSearcher : ITextSearcher
	{
		ISessionProvider sessionProvider;

		public LuceneTextSearcher(ISessionProvider sessionProvider)
		{
			this.sessionProvider = sessionProvider;
		}

		#region ITextSearcher Members

		public IEnumerable<ContentItem> Search(ContentItem ancestor, string query, int skip, int take, out int totalRecords)
		{
			var q = sessionProvider.OpenSession.FullText().CreateFullTextQuery<ContentItem>(FormatQuery(query));
			totalRecords = q.ResultSize;
			q.SetFirstResult(skip)
				.SetMaxResults(take);
			return q.List<ContentItem>();
		}

		static string FormatQuery(string text)
		{
			return string.Format("Title:({0}) or Details.StringValue:({0})", text);
		}
		#endregion
	}
}
