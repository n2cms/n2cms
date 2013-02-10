using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver.Builders;
using N2.Collections;
using N2.Definitions;
using N2.Persistence.Proxying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.MongoDB
{
	public abstract class ContentClassMapFactory
	{
		public abstract BsonClassMap Create(Definitions.ItemDefinition definition, IEnumerable<Definitions.ItemDefinition> allDefinitions, IProxyFactory proxies, MongoDatabaseProvider database);

		protected BsonClassMap Create<T>(Definitions.ItemDefinition definition, IEnumerable<Definitions.ItemDefinition> allDefinitions, IProxyFactory proxies, MongoDatabaseProvider database)
			where T : ContentItem
		{
			return new BsonClassMap<T>(cm =>
				{
					cm.SetDiscriminator(definition.Discriminator);

					BsonSerializer.RegisterDiscriminatorConvention(definition.ItemType, new IgnoreProxyTypeDiscriminatorConvention());

					foreach (var type in allDefinitions.Select(d => d.ItemType).Where(t => definition.ItemType.IsAssignableFrom(t)))
						cm.AddKnownType(type);

					cm.AutoMap();

					cm.SetCreator(() => 
						 {
							 try
							 {
								 return database.IdentityMap.Current
									 ?? CreateItem<T>(definition, proxies, database);
							 }
							 finally
							 {
								 database.IdentityMap.Current = null;
							 }
						 });

					foreach (var p in definition.Properties.Values
						.Where(p => p.Info != null)
						.Where(p => typeof(ContentItem).IsAssignableFrom(p.Info.PropertyType))
						.Where(p => p.Info.DeclaringType == definition.ItemType))
					{
						if (p.Info.CanRead && p.Info.CanWrite)
							cm.GetMemberMap(p.Name).SetSerializer(new ContentReferenceSerializer(database));
						else
							cm.UnmapProperty(p.Name);
					}
				});
		}

		private static T CreateItem<T>(ItemDefinition definition, IProxyFactory proxies, MongoDatabaseProvider database) where T : ContentItem
		{
			object item = proxies.Create(typeof(T).Name, 0) 
				?? definition.CreateInstance(null, applyDefaultValues: false);
			return (T)item;
		}
	}

	public class ContentClassMapFactory<T> : ContentClassMapFactory
		where T: ContentItem
	{
		public override BsonClassMap Create(Definitions.ItemDefinition definition, IEnumerable<Definitions.ItemDefinition> allDefinitions, IProxyFactory proxies, MongoDatabaseProvider database)
		{
			return Create<T>(definition, allDefinitions, proxies, database);
		}
	}
}
