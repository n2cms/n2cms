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
	public class ContentReferenceSerializer : BsonBaseSerializer
	{
		private MongoDatabaseProvider database;

		public ContentReferenceSerializer(MongoDatabaseProvider database)
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
			return database.GetCollection<ContentItem>().FindOne(Query.EQ("_id", id));
		}

		public override void Serialize(BsonWriter bsonWriter, Type nominalType, object value, IBsonSerializationOptions options)
		{
			bsonWriter.WriteInt32(((ContentItem)value).ID);
		}
	}
}
