using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.MongoDB
{
	public abstract class ContentClassMapFactory
	{
		public abstract BsonClassMap Create(Definitions.ItemDefinition definition, IEnumerable<Definitions.ItemDefinition> allDefinitions);

		protected BsonClassMap Create<T>(Definitions.ItemDefinition definition, IEnumerable<Definitions.ItemDefinition> allDefinitions)
			where T : ContentItem
		{
			return new BsonClassMap<T>(cm =>
				{
					cm.SetDiscriminator(definition.Discriminator);

					foreach (var type in allDefinitions.Select(d => d.ItemType).Where(t => definition.ItemType.IsAssignableFrom(t)))
						cm.AddKnownType(type);

					cm.AutoMap();

					//cm.MapIdProperty(ci => ci.ID).SetIdGenerator(new IntIdGenerator());

					//cm.UnmapProperty("Children");
					//cm.UnmapProperty("DetailCollections");
					//cm.UnmapProperty("VersionOf");
				});
		}
	}

	public class ContentClassMapFactory<T> : ContentClassMapFactory
		where T: ContentItem
	{
		public override BsonClassMap Create(Definitions.ItemDefinition definition, IEnumerable<Definitions.ItemDefinition> allDefinitions)
		{
			return Create<T>(definition, allDefinitions);
		}
	}
}
