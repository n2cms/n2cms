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
	[Service(typeof(ITextSearcher), Replaces = typeof(FindingTextSearcher), Configuration = "lucene")]
	public class LuceneSearcher : ITextSearcher
	{
		private readonly Engine.Logger<LuceneSearcher> logger;
		LuceneAccesor accessor;
		IPersister persister;

		public LuceneSearcher(LuceneAccesor accessor, IPersister persister)
		{
			this.accessor = accessor;
			this.persister = persister;
		}

		#region ITextSearcher Members

		public Result Search(N2.Persistence.Search.Query query)
		{
			if (!query.IsValid())
				return Result.Empty;

			var s = accessor.GetSearcher();
			try
			{
				var q = CreateQuery(query);
				//new TopFieldDocCollector(s.GetIndexReader(), new Sort(
				TopDocs hits;
				if (string.IsNullOrEmpty(query.SortField))
					hits = s.Search(q, query.SkipHits + query.TakeHits);
				else
					hits = s.Search(
						query: q,
						filter: null,
						n: query.SkipHits + query.TakeHits,
						sort: new Sort(
							query.SortFields.Select(
								field => new SortField(field.SortField, GetSortFieldType(field.SortField), field.SortDescending)).ToArray()));

				var result = new Result();
				result.Total = hits.totalHits;
				var resultHits = hits.scoreDocs.Skip(query.SkipHits).Take(query.TakeHits).Select(hit =>
				{
					var doc = s.Doc(hit.doc);
					int id = int.Parse(doc.Get("ID"));
					ContentItem item = persister.Get(id);
					return new Hit { Content = item, Score = hit.score };
				}).Where(h => h.Content != null).ToList();
				result.Hits = resultHits;
				result.Count = resultHits.Count;
				return result;
			}
			finally
			{
				//s.Close();
			}
		}

		private int GetSortFieldType(string sortField)
		{
			switch(sortField)
			{
				case LuceneIndexer.Properties.ID:
				case LuceneIndexer.Properties.SortOrder:
					return SortField.INT;
				default:
					return SortField.STRING;
			}
		}

		protected virtual Lucene.Net.Search.Query CreateQuery(N2.Persistence.Search.Query query)
		{
			var q = "";
			if (!string.IsNullOrEmpty(query.Text))
				q = query.OnlyPages.HasValue
					 ? string.Format("+(Title:({0})^4 Text:({0}) PartsText:({0}))", query.Text)
					 : string.Format("+(Title:({0})^4 Text:({0}))", query.Text);

			if (query.Ancestor != null)
				q += string.Format(" +Trail:{0}*", Utility.GetTrail(query.Ancestor));
			if (query.OnlyPages.HasValue)
				q += string.Format(" +IsPage:{0}", query.OnlyPages.Value.ToString().ToLower());
			if (query.Roles != null)
				q += string.Format(" +Roles:(Everyone {0})", string.Join(" ", query.Roles.ToArray()));
			if (query.Types != null)
				q += string.Format(" +Types:({0})", string.Join(" ", query.Types.Select(t => t.Name).ToArray()));
			if (query.LanguageCode != null)
				q += string.Format(" +Language:({0})", query.LanguageCode);
			if (query.Exclution != null)
				q += string.Format(" -({0})", CreateQuery(query.Exclution));
			if (query.Intersection != null)
				q = string.Format("+({0}) +({1})", q, CreateQuery(query.Intersection));
			if (query.Union != null)
				q = string.Format("({0}) ({1})", q, CreateQuery(query.Union));
			if (query.Details.Count > 0)
				foreach (var kvp in query.Details)
				{
					if (LuceneIndexer.Properties.All.Contains(kvp.Key))
						q += string.Format(" +{0}:({1})", kvp.Key, kvp.Value);
					else
						q += string.Format(" +Detail.{0}:({1})", kvp.Key, kvp.Value);
				}
			logger.Debug("CreateQuery: " + q);

			return accessor.GetQueryParser().Parse(q);
		}
		#endregion
	}
}
