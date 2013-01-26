using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver.Builders;
using N2.Collections;
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

		public ContentSerializer(Type type, MongoDatabaseProvider database)
		{
			this.database = database;
			classMap = BsonClassMap.LookupClassMap(type);
			serializer = new BsonClassMapSerializer(classMap);
		}

		public object Deserialize(BsonReader bsonReader, Type nominalType, Type actualType, IBsonSerializationOptions options)
		{
			return Inject(serializer.Deserialize(bsonReader, nominalType, actualType, options));
		}

		private object Inject(object value)
		{
			var item = (ContentItem)value;
			var collection = database.GetCollection<ContentItem>();
			var ids = item.AncestralTrail.Split('/').Where(id => !string.IsNullOrEmpty(id)).Select(id => BsonInt32.Create(int.Parse(id)));
			var parents = collection.Find(Query.In("_id", ids));
			ContentItem last = item;
			foreach (var parent in parents.OrderByDescending(p => p.AncestralTrail.Length))
			{
				last.Parent = parent;
				last = parent;
			}
			item.Children = new ItemList(() => collection.Find(Query.EQ("AncestralTrail", item.GetTrail())));
			return item;
		}

		public object Deserialize(BsonReader bsonReader, Type nominalType, IBsonSerializationOptions options)
		{
			return Inject(serializer.Deserialize(bsonReader, nominalType, options));
		}

		public IBsonSerializationOptions GetDefaultSerializationOptions()
		{
			return serializer.GetDefaultSerializationOptions();
		}

		public void Serialize(BsonWriter bsonWriter, Type nominalType, object value, IBsonSerializationOptions options)
		{
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
