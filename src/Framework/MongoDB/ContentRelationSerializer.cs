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
    public class ContentRelationSerializer : BsonBaseSerializer
    {
        private MongoDatabaseProvider database;

        public ContentRelationSerializer(MongoDatabaseProvider database)
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
                return ContentRelation.Empty;
            
            return new ContentRelation(id, Get);
        }

        private ContentItem Get(object id)
        {
            return database.IdentityMap.GetOrCreate(id, (i) => database.GetCollection<ContentItem>().FindOne(Query.EQ("_id", (int)i)));
        }

        public override void Serialize(BsonWriter bsonWriter, Type nominalType, object value, IBsonSerializationOptions options)
        {
            var relation = value as ContentRelation;

            if (relation == null || !relation.ID.HasValue || relation.ID.Value == 0)
                bsonWriter.WriteInt32(0);
            else
                bsonWriter.WriteInt32(relation.ID.Value);
        }
    }
}
