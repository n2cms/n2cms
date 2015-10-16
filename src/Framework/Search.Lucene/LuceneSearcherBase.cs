using Lucene.Net.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.Search
{
    public abstract class LuceneSearcherBase<T> where T : class
    {
        private readonly Engine.Logger<LuceneSearcherBase<T>> logger;
        LuceneAccesor accessor;

        public LuceneSearcherBase(LuceneAccesor accessor)
        {
            this.accessor = accessor;
        }

        public Result<T> Search(N2.Persistence.Search.Query query)
        {
			if (!query.IsValid())
			{
				logger.Warn("Invalid query");
				return Result<T>.Empty;
			}

            var luceneQuery = query.ToLuceneQuery();
			logger.InfoFormat("Prepared lucene query {0}", luceneQuery);

            var q = accessor.GetQueryParser().Parse(luceneQuery);
            var s = accessor.GetSearcher();
            
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

            return CreateResults(query, s, hits);
        }

        protected abstract Result<T> CreateResults(N2.Persistence.Search.Query query, IndexSearcher s, TopDocs hits);

        private int GetSortFieldType(string sortField)
        {
            switch (sortField)
            {
                case TextExtractor.Properties.ID:
                case TextExtractor.Properties.SortOrder:
                    return SortField.INT;
                default:
                    return SortField.STRING;
            }
        }
    }
}
