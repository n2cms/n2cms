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
    [Service(typeof(ILightweightSearcher), Replaces = typeof(FindingContentSearcher), Configuration = "Lucene")]
    public class LuceneLightweightSearcher : LuceneSearcherBase<LightweightHitData>, ILightweightSearcher
    {
        LuceneAccesor accessor;
		private readonly Engine.Logger<LuceneLightweightSearcher> logger;

        public LuceneLightweightSearcher(LuceneAccesor accessor)
            : base(accessor)
        {
            this.accessor = accessor;
        }

        protected override Result<LightweightHitData> CreateResults(N2.Persistence.Search.Query query, IndexSearcher s, TopDocs hits)
        {
			logger.InfoFormat("Creating results for query {0} and {1} hits", query.Text, hits.TotalHits);

            var result = new Result<LightweightHitData>();
            result.Total = hits.TotalHits;
            var resultHits = hits.ScoreDocs.Skip(query.SkipHits).Take(query.TakeHits).Select(hit =>
            {
                var doc = s.Doc(hit.Doc);
                int id = int.Parse(doc.Get("ID"));
                return new Hit<LightweightHitData>
                {
                    Content = new LightweightHitData
                    {
                        ID = id,
                        AlteredPermissions = (Security.Permission)int.Parse(doc.Get("AlteredPermissions")),
                        State = (ContentState)int.Parse(doc.Get("State")),
                        Visible = Convert.ToBoolean(doc.Get("Visible")),
                        AuthorizedRoles = doc.Get("Roles").Split(' '),
                        Path = doc.Get("Path")
                    },
                    Title = doc.Get("Title"),
                    Url = doc.Get("Url"),
                    Score = hit.Score
                };
            }).ToList();
            result.Hits = resultHits;
            result.Count = resultHits.Count;
            return result;
        }
    }
}
