using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.MongoDB
{
    public class RelationValueAccessorSerializer<T> : IBsonSerializer
    {
        private ObjectSerializer serializer = new ObjectSerializer();
        private MongoDatabaseProvider databaseProvider;

        public RelationValueAccessorSerializer(MongoDatabaseProvider databaseProvider)
        {
            this.databaseProvider = databaseProvider;
        }

        private T Get(object id)
        {
            return databaseProvider.GetCollection<T>().FindOne(Query.EQ("_id", (int)id));
        }

        public object Deserialize(BsonReader bsonReader, Type nominalType, Type actualType, IBsonSerializationOptions options)
        {
            serializer.Deserialize(bsonReader, typeof(object), typeof(object), options);
            return (Func<object, T>)Get;
        }

        public object Deserialize(BsonReader bsonReader, Type nominalType, IBsonSerializationOptions options)
        {
            serializer.Deserialize(bsonReader, typeof(object), options);
            return (Func<object, T>)Get;
        }

        public IBsonSerializationOptions GetDefaultSerializationOptions()
        {
            return null;
        }

        public void Serialize(BsonWriter bsonWriter, Type nominalType, object value, IBsonSerializationOptions options)
        {
            serializer.Serialize(bsonWriter, typeof(object), null, options);
        }
    }
}
