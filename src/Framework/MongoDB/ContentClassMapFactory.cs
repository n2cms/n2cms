using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver.Builders;
using N2.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.MongoDB
{
	public abstract class ContentClassMapFactory
	{
		public abstract BsonClassMap Create(Definitions.ItemDefinition definition, IEnumerable<Definitions.ItemDefinition> allDefinitions, ContentActivator activator, MongoDatabaseProvider database);

		protected BsonClassMap Create<T>(Definitions.ItemDefinition definition, IEnumerable<Definitions.ItemDefinition> allDefinitions, ContentActivator activator, MongoDatabaseProvider database)
			where T : ContentItem
		{
			return new BsonClassMap<T>(cm =>
				{
					cm.SetDiscriminator(definition.Discriminator);

					foreach (var type in allDefinitions.Select(d => d.ItemType).Where(t => definition.ItemType.IsAssignableFrom(t)))
						cm.AddKnownType(type);

					cm.AutoMap();

					cm.SetCreator(() => CreateItem<T>(activator, database));

					//cm.MapIdProperty(ci => ci.ID).SetIdGenerator(new IntIdGenerator());

					//cm.UnmapProperty("Children");
					//cm.UnmapProperty("DetailCollections");
					//cm.UnmapProperty("VersionOf");
				});
		}

		private static T CreateItem<T>(ContentActivator activator, MongoDatabaseProvider database) where T : ContentItem
		{
			var item = activator.CreateInstance<T>(parentItem: null, templateKey: null, asProxy: true);
			return item;
		}
	}

	public class ContentClassMapFactory<T> : ContentClassMapFactory
		where T: ContentItem
	{
		public override BsonClassMap Create(Definitions.ItemDefinition definition, IEnumerable<Definitions.ItemDefinition> allDefinitions, ContentActivator activator, MongoDatabaseProvider database)
		{
			return Create<T>(definition, allDefinitions, activator, database);
		}
	}
}
