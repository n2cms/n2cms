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
    [Service(typeof(IContentSearcher), Replaces = typeof(FindingContentSearcher), Configuration = "Lucene")]
    public class LuceneContentSearcher : LuceneSearcherBase<ContentItem>, IContentSearcher
    {
        LuceneAccesor accessor;
        IPersister persister;

        public LuceneContentSearcher(LuceneAccesor accessor, IPersister persister)
            : base(accessor)
        {
            this.accessor = accessor;
            this.persister = persister;
        }

        protected override Result<ContentItem> CreateResults(Query query, IndexSearcher s, TopDocs hits)
        {
            var result = new Result<ContentItem>();
            result.Total = hits.TotalHits;
            var resultHits = hits.ScoreDocs.Skip(query.SkipHits).Take(query.TakeHits).Select(hit =>
            {
                var doc = s.Doc(hit.Doc);
                int id = int.Parse(doc.Get("ID"));
                return new LazyHit<ContentItem> { ID = id, Title = doc.Get("Title"), Url = doc.Get("Url"), ContentAccessor = persister.Get, Score = hit.Score };
            }).ToList();
            result.Hits = resultHits;
            result.Count = resultHits.Count;
            return result;
        }
    }
}
