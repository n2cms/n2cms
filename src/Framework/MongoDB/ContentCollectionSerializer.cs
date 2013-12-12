using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using N2.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.MongoDB
{
    public class ContentCollectionSerializer<T> : BsonBaseSerializer, IBsonArraySerializer
        where T : class, INameable
    {
        EnumerableSerializer<T> serializer = new EnumerableSerializer<T>();

        public Type CollectionType { get; set; }

        public ContentCollectionSerializer()
        {
            CollectionType = typeof(ContentList<T>);
        }

        public override object Deserialize(BsonReader bsonReader, Type nominalType, Type actualType, IBsonSerializationOptions options)
        {
            return serializer.Deserialize(bsonReader, nominalType, CollectionType, options);
        }

        public override object Deserialize(BsonReader bsonReader, Type nominalType, IBsonSerializationOptions options)
        {
            return serializer.Deserialize(bsonReader, nominalType, options);
        }

        public override IBsonSerializationOptions GetDefaultSerializationOptions()
        {
            return serializer.GetDefaultSerializationOptions();
        }

        public override void Serialize(BsonWriter bsonWriter, Type nominalType, object value, IBsonSerializationOptions options)
        {
            serializer.Serialize(bsonWriter, nominalType, value, options);
        }

        public BsonSerializationInfo GetItemSerializationInfo()
        {
            return new BsonSerializationInfo("item", this, typeof(T), GetDefaultSerializationOptions());
        }
    }
}
