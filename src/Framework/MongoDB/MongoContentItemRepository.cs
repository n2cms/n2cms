using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver.Linq;
using MongoDB.Driver.Builders;
using MongoDB.Bson;
using MongoDB.Driver;
using N2.Engine;
using N2.Persistence.NH;

namespace N2.Persistence.MongoDB
{
	[Service(typeof(IContentItemRepository), Configuration = "mongo")]
	[Service(typeof(IRepository<ContentItem>), Configuration = "mongo", Replaces = typeof(ContentItemRepository))]
    public class MongoContentItemRepository : MongoDbRepository<ContentItem>, IContentItemRepository
    {
		public MongoContentItemRepository(MongoDatabaseProvider provider)
			: base(provider)
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
				.Select(doc => new DiscriminatorCount { Discriminator = ((BsonArray)doc["_id"]).Last().ToString(), Count = (int)doc["value"].ToDouble() })
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
			yield break;
        }

        public int RemoveReferencesToRecursive(ContentItem target)
        {
			return 0;
        }

        public void DropDatabase()
        {
			foreach (var collectionName in Provider.Database.GetCollectionNames().Where(cn => !cn.StartsWith("system.")))
				Provider.Database.DropCollection(collectionName);
        }

		protected override int GetEntityId(ContentItem entity)
		{
			return entity.ID;
		}
    }
}
