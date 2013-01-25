using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using N2.Definitions;
using N2.Details;
using N2.Edit.Versioning;
using N2.Engine;
using N2.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.MongoDB
{
	[Service(Configuration = "mongo")]
	public class MongoDatabaseProvider
	{
		public MongoDatabaseProvider(Configuration.ConfigurationManagerWrapper config, IDefinitionManager definitions)
		{
			Register(definitions);
			Connect(config);
		}

		private void Connect(Configuration.ConfigurationManagerWrapper config)
		{
			var connectionString = config.GetConnectionString();
			var client = new MongoClient(connectionString);
			var server = client.GetServer();
			Database = server.GetDatabase(config.Sections.Database.TablePrefix + "cms");
		}

		static bool isRegistered = false;
		private static void Register(IDefinitionManager definitions)
		{
			if (isRegistered)
				return;
			isRegistered = true;

			var conventions = new ConventionProfile();
			conventions.SetIgnoreIfNullConvention(new AlwaysIgnoreIfNullConvention());
			BsonClassMap.RegisterConventions(conventions, t => true);

			BsonSerializer.RegisterSerializationProvider(new ContentSerializationProvider());

			BsonClassMap.RegisterClassMap<AuthorizedRole>();
			BsonClassMap.RegisterClassMap<ContentDetail>(cm =>
			{
				cm.AutoMap();
				cm.UnmapProperty(cd => cd.EnclosingCollection);
				cm.UnmapProperty(cd => cd.EnclosingItem);
			});
			BsonClassMap.RegisterClassMap<DetailCollection>(cm =>
			{
				cm.AutoMap();
				cm.UnmapProperty(cd => cd.EnclosingItem);
			});
			BsonClassMap.RegisterClassMap<ContentVersion>();
			var map = BsonClassMap.RegisterClassMap<ContentItem>(cm =>
			{
				cm.AutoMap();
				cm.MapIdProperty(ci => ci.ID).SetIdGenerator(new IntIdGenerator());
				cm.UnmapProperty(ci => ci.Children);
				//cm.UnmapProperty(ci => ci.Details);
				cm.UnmapProperty(ci => ci.DetailCollections);
				cm.UnmapProperty(ci => ci.Parent);
				cm.UnmapProperty(ci => ci.VersionOf);
				cm.SetIsRootClass(isRootClass: true);
			});
			
			var allDefinitions = definitions.GetDefinitions().ToList();
			foreach (var definition in definitions.GetDefinitions())
			{
				var factory = (ContentClassMapFactory)Activator.CreateInstance(typeof(ContentClassMapFactory<>).MakeGenericType(definition.ItemType));
				BsonClassMap.RegisterClassMap(factory.Create(definition, allDefinitions));
			}
		}

		public MongoDatabase Database { get; set; }
	}
}
