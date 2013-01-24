using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver.Linq;
using MongoDB.Driver.Builders;
using MongoDB.Bson;
using MongoDB.Driver;

namespace N2.Persistence.MongoDB
{
    public class MongoContentItemRepository : MongoDbRepository<ContentItem>, IContentItemRepository
    {
		public MongoContentItemRepository(Configuration.ConfigurationManagerWrapper config)
			: base(config)
		{
		}

        public IEnumerable<DiscriminatorCount> FindDescendantDiscriminators(ContentItem ancestor)
        {
			var map = "function() { emit(this._t, 1); }";
			var reduce = "function(key, emits) { var total = 0; for (var i in emits) { total += emits[i]; }; return total; }";
			var results = ancestor != null
				? GetCollection().MapReduce(ItselfOrBelow(ancestor), map, reduce)
				: GetCollection().MapReduce(map, reduce);

			return results.InlineResults
				.Where(doc => !doc["_id"].IsBsonNull)
				.Select(doc => new DiscriminatorCount { Discriminator = (string)doc["_id"], Count = (int)doc["value"].ToDouble() })
				.OrderByDescending(dc => dc.Count)
				.ToList();
        }

		private static IMongoQuery ItselfOrBelow(ContentItem ancestor)
		{
			return Query.Or(Query.EQ("_id", ancestor.ID), Below(ancestor));
		}

		private static IMongoQuery Below(ContentItem ancestor)
		{
			return Query.Matches("AncestralTrail", "^" + ancestor.GetTrail().Replace("/", "\\/") + ".*");
		}

        public IEnumerable<ContentItem> FindDescendants(ContentItem ancestor, string discriminator)
        {
			if (ancestor == null)
				return GetCollection().Find(Query.EQ("_t", discriminator));
			else
				return GetCollection().Find(Query.And(ItselfOrBelow(ancestor), Query.EQ("_t", discriminator)));
        }

        public IEnumerable<ContentItem> FindReferencing(ContentItem linkTarget)
        {
            throw new NotImplementedException();
        }

        public int RemoveReferencesToRecursive(ContentItem target)
        {
            throw new NotImplementedException();
        }

        public ICollection<ContentItem> FindAll()
        {
            return GetCollection().AsQueryable().ToList();
        }

        public void DropDatabase()
        {
			foreach (var cn in Database.GetCollectionNames().Where(cn => !cn.StartsWith("system.")))
				Database.DropCollection(cn);
        }
    }
}
