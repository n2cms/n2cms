using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;
using N2.Persistence.Finder;

namespace N2.Persistence.Search
{
    [Service(typeof(ITextSearcher))]
    [Obsolete("Use IContentSearcher")]
    public class BackwardsCompatibleSearcher : ITextSearcher
    {
        private IContentSearcher searcher;
        
        public BackwardsCompatibleSearcher(IContentSearcher searcher)
        {
            this.searcher = searcher;
        }

        public Result<ContentItem> Search(Query query)
        {
            return searcher.Search(query);
        }
    }


    /// <summary>
    /// Searches for text using the finder API which results in LIKE database queries.
    /// </summary>
    [Service(typeof(IContentSearcher))]
    [Service(typeof(ILightweightSearcher))]
    public class FindingContentSearcher : IContentSearcher, ILightweightSearcher
    {
		private IContentItemRepository repository;
		//IItemFinder finder;

		//public FindingContentSearcher(IItemFinder finder)
        public FindingContentSearcher(IContentItemRepository repository)
        {
            //this.finder = finder;
			this.repository = repository;
        }

        #region ITextSearcher Members

        /// <summary>Searches for items below an ancestor that matches the given query.</summary>
        /// <param name="ancestor">The ancestor below which the results should be found.</param>
        /// <param name="query">The query text.</param>
        /// <param name="skip">A number of items to skip.</param>
        /// <param name="take">A number of items to take.</param>
        /// <returns>An enumeration of items matching the search query.</returns>
        public IEnumerable<ContentItem> Search(string ancestor, string query, int skip, int take, bool? onlyPages, string[] types, out int totalRecords)
        {
			var q = new ParameterCollection();
			if (ancestor != null)
			{
				q.Add(Parameter.Like("AncestralTrail", ancestor + "%") & Parameter.IsNotNull("Title"));
			}
			
			if (!string.IsNullOrEmpty(query))
			{
				var words = query.Split(' ').Where(w => w.Length > 0).Select(w => "%" + w.Trim('*') + "%");
				q.Add(new ParameterCollection(Operator.Or, words.Select(word => (Parameter.Like("Title", word) | Parameter.Like(null, word).Detail()))));
			}

			if (onlyPages.HasValue)
				if (onlyPages.Value)
					q.Add(Parameter.IsNull("ZoneName"));
				else
					q.Add(Parameter.IsNotNull("ZoneName"));

			if (types != null)
				q.Add(Parameter.In("class", types));

			totalRecords = (int)repository.Count(q);
			return repository.Find(q.Skip(skip).Take(take));
			
			//var q = finder.Where.AncestralTrail.Like(ancestor + "%")
			//	.And.Title.IsNull(false);

			//var words = query.Split(' ').Where(w => w.Length > 0).Select(w => "%" + w.Trim('*') + "%");
			//foreach (var word in words)
			//{
			//	q = q.And.OpenBracket()
			//		.Title.Like(word)
			//		.Or.Detail().Like(word)
			//	.CloseBracket();
			//}
			//if (onlyPages.HasValue)
			//	q = q.And.OpenBracket()
			//		.ZoneName.IsNull(onlyPages.Value)
			//		.Or.ZoneName.Eq("")
			//	.CloseBracket();
			//if (types != null)
			//	q = q.And.Property("class").In(types);

			//totalRecords = q.Count();
			//return q.FirstResult(skip).MaxResults(take).Select();
        }

        public Result<ContentItem> Search(Query query)
        {
            var result = new Result<ContentItem>();
            int total;
            result.Hits = Search(query.Ancestor, query.Text, query.SkipHits, query.TakeHits, query.OnlyPages, query.Types, out total)
				.Select(i => new Hit<ContentItem> { Title = i.Title, Url = i.Url, Content = i, Score = (i.Title != null && query.Text != null && i.Title.ToLower().Contains(query.Text.ToLower())) ? 1 : .5 });
            result.Total = total;
            return result;
        }

        #endregion

        Result<LightweightHitData> ISearcher<LightweightHitData>.Search(Query query)
        {
            var result = Search(query);
            return new Result<LightweightHitData>
            {
                Count = result.Count,
                Total = result.Total,
                Hits = result.Hits.Select(h => new Hit<LightweightHitData>
                {
                    Score = h.Score,
                    Title = h.Title,
                    Url = h.Url,
                    Content = new LightweightHitData 
                    { 
                        ID = h.Content.ID,
                        AlteredPermissions = h.Content.AlteredPermissions,
                        AuthorizedRoles = h.Content.AuthorizedRoles.Any() ? h.Content.AuthorizedRoles.Select(ar => ar.Role).ToArray() : new[] { "Everyone" }, 
                        State = h.Content.State,
                        Visible = h.Content.Visible,
                        Path = h.Content.Path 
                    }
                }).ToList()
            };
        }
    }
}
