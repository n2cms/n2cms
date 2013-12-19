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
    public class ContentItemReferenceSerializer : BsonBaseSerializer
    {
        private MongoDatabaseProvider database;

        public ContentItemReferenceSerializer(MongoDatabaseProvider database)
        {
            this.database = database;
        }

        public override object Deserialize(BsonReader bsonReader, Type nominalType, Type actualType, IBsonSerializationOptions options)
        {
            return Deserialize(bsonReader, nominalType, options);
        }

        public override object Deserialize(BsonReader bsonReader, Type nominalType, IBsonSerializationOptions options)
        {
            var id = bsonReader.ReadInt32();
            if (id == 0)
                return null;
            return database.IdentityMap.GetOrCreate(id, (i) => database.GetCollection<ContentItem>().FindOne(Query.EQ("_id", i)));
        }

        public override void Serialize(BsonWriter bsonWriter, Type nominalType, object value, IBsonSerializationOptions options)
        {
            if (value == null)
                bsonWriter.WriteInt32(0);
            else
                bsonWriter.WriteInt32(((ContentItem)value).ID);
        }
    }
}
