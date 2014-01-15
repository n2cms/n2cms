using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver.Builders;
using N2.Collections;
using N2.Persistence.Proxying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.MongoDB
{
    public class ContentSerializer : IBsonSerializer, IBsonIdProvider, IBsonDocumentSerializer
    {
        private BsonClassMap classMap;
        private BsonClassMapSerializer serializer;
        private MongoDatabaseProvider database;
        private IProxyFactory proxies;

        public ContentSerializer(Type type, MongoDatabaseProvider database, IProxyFactory proxies)
        {
            this.database = database;
            classMap = BsonClassMap.LookupClassMap(type);
            serializer = new BsonClassMapSerializer(classMap);
            this.proxies = proxies;
        }

        public object Deserialize(BsonReader bsonReader, Type nominalType, Type actualType, IBsonSerializationOptions options)
        {
            var doc = BsonDocument.ReadFrom(bsonReader);
            var id = Convert.ToInt32(doc["_id"]);
            //return Inject(serializer.Deserialize(bsonReader, nominalType, actualType, options));
            //return Inject(serializer.Deserialize(BsonReader.Create(doc), nominalType, actualType, options));

            string key = MongoIdentityMap.GetKey<ContentItem>(id);

            var item = database.IdentityMap.Get(key);
            var type = actualType ?? BsonSerializer.LookupActualType(nominalType, doc["_t"].AsBsonArray.Last());
            if (item == null)
            {
                item = BsonClassMap.LookupClassMap(type).CreateInstance();
                database.IdentityMap.Set(key, item);
            }
            else
            {
                if (((ContentItem)item).State != ContentState.None)
                {
                    return item;
                }
            }

            try
            {
                database.IdentityMap.Current = item;
                var deserializedItem = (actualType == null)
                    ? serializer.Deserialize(BsonReader.Create(doc), nominalType, options)
                    : serializer.Deserialize(BsonReader.Create(doc), nominalType, actualType, options);
                return Inject(deserializedItem);
            }
            finally
            {
                database.IdentityMap.Current = null;
            }
        }

        public object Deserialize(BsonReader bsonReader, Type nominalType, IBsonSerializationOptions options)
        {
            return serializer.Deserialize(bsonReader, nominalType, options);
        }

        private ContentItem Inject(object value)
        {
            var item = (ContentItem)value;

            //SetAncestors(item);

            SetChildren(item);

            proxies.OnLoaded(item);
            return item;
        }

        private void SetChildren(ContentItem item)
        {
            item.Children = new ItemList(() => database.GetCollection<ContentItem>().Find(Query.EQ("AncestralTrail", item.GetTrail())));
        }

        private void SetAncestors(ContentItem item)
        {
            var ids = item.AncestralTrail.Split('/').Where(id => !string.IsNullOrEmpty(id)).Select(id => BsonInt32.Create(int.Parse(id))).ToList();
            if (ids.Count == 0)
                return;

            var parents = database.GetCollection<ContentItem>().Find(Query.In("_id", ids)).OrderByDescending(p => p.AncestralTrail.Length).ToList();
            for (int i = 1; i < parents.Count; i++)
            {
                parents[i - 1].Parent = parents[i];
            }
            item.Parent = parents[0];
        }

        public IBsonSerializationOptions GetDefaultSerializationOptions()
        {
            return serializer.GetDefaultSerializationOptions();
        }

        public void Serialize(BsonWriter bsonWriter, Type nominalType, object value, IBsonSerializationOptions options)
        {
            proxies.OnSaving(value);
            serializer.Serialize(bsonWriter, nominalType, value, options);
        }

        public bool GetDocumentId(object document, out object id, out Type idNominalType, out IIdGenerator idGenerator)
        {
            return serializer.GetDocumentId(document, out id, out idNominalType, out idGenerator);
        }

        public void SetDocumentId(object document, object id)
        {
            serializer.SetDocumentId(document, id);
        }

        public BsonSerializationInfo GetMemberSerializationInfo(string memberName)
        {
            return serializer.GetMemberSerializationInfo(memberName);
        }
    }
}
