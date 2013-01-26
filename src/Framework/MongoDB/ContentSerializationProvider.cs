using MongoDB.Bson.Serialization;
using N2.Details;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.MongoDB
{
	public class ContentSerializationProvider : IBsonSerializationProvider
	{
		private MongoDatabaseProvider database;

		public ContentSerializationProvider(MongoDatabaseProvider database)
		{
			this.database = database;
		}

		public IBsonSerializer GetSerializer(Type type)
		{
			if (typeof(IEnumerable<ContentDetail>).IsAssignableFrom(type))
			{
				return new ContentCollectionSerializationProvider<ContentDetail>();
			}
			if (typeof(IEnumerable<DetailCollection>).IsAssignableFrom(type))
			{
				return new ContentCollectionSerializationProvider<DetailCollection>();
			}
			if (typeof(ContentItem).IsAssignableFrom(type))
			{
				return new ContentSerializer(type, database);
			}

			return null;
		}
	}
}
