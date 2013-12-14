using MongoDB.Bson.Serialization;
using N2.Details;
using N2.Persistence.Proxying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.MongoDB
{
    public class ContentSerializationProvider : IBsonSerializationProvider
    {
        private MongoDatabaseProvider database;
        private IProxyFactory proxies;

        public ContentSerializationProvider(MongoDatabaseProvider database, IProxyFactory proxies)
        {
            this.database = database;
            this.proxies = proxies;
        }

        public IBsonSerializer GetSerializer(Type type)
        {
            if (typeof(IEnumerable<ContentDetail>).IsAssignableFrom(type))
            {
                return new ContentCollectionSerializer<ContentDetail>();
            }
            if (typeof(IEnumerable<DetailCollection>).IsAssignableFrom(type))
            {
                return new ContentCollectionSerializer<DetailCollection>();
            }
            if (typeof(ContentItem).IsAssignableFrom(type))
            {
                return new ContentSerializer(type, database, proxies);
            }
            if (typeof(DetailCollection) == type)
            {
                return new BsonClassMapSerializer(BsonClassMap.LookupClassMap(type));
            }

            return null;
        }
    }
}
