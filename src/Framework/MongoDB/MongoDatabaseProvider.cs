using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
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
		public MongoDatabaseProvider(Configuration.ConfigurationManagerWrapper config, IDefinitionProvider[] definitionProviders, ContentActivator activator)
		{
			Register(definitionProviders, activator);
			Connect(config);
		}

		public MongoDatabase Database { get; set; }

		private void Connect(Configuration.ConfigurationManagerWrapper config)
		{
			var connectionString = config.GetConnectionString();
			var settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
			settings.ConnectTimeout = TimeSpan.FromSeconds(10);
			var client = new MongoClient(settings);
			var server = client.GetServer();
			Database = server.GetDatabase(config.Sections.Database.TablePrefix + "cms");
			GetCollection<ContentItem>().EnsureIndex("Details.Name", "Details.LinkValue");
		}

		static bool isRegistered = false;
		private void Register(IDefinitionProvider[] definitionProviders, ContentActivator activator)
		{
			if (isRegistered)
				return;
			isRegistered = true;

			var conventions = new ConventionProfile();
			conventions.SetIgnoreIfNullConvention(new AlwaysIgnoreIfNullConvention());
			conventions.SetMemberFinderConvention(new IgnoreUnderscoreMemberFinderConvention());
			BsonClassMap.RegisterConventions(conventions, t => true);

			BsonSerializer.RegisterSerializationProvider(new ContentSerializationProvider(this));

			BsonClassMap.RegisterClassMap<AuthorizedRole>(cm =>
			{
				cm.AutoMap();
				cm.UnmapProperty(cd => cd.ID);
				cm.UnmapField(cd => cd.EnclosingItem);
			});
			BsonClassMap.RegisterClassMap<ContentDetail>(cm =>
			{
				cm.AutoMap();
				cm.UnmapProperty(cd => cd.ID);
				cm.UnmapProperty(cd => cd.EnclosingCollection);
				cm.UnmapProperty(cd => cd.EnclosingItem);
				cm.UnmapProperty(cd => cd.Value);
				cm.UnmapProperty(cd => cd.LinkedItem);
			});
			BsonClassMap.RegisterClassMap<DetailCollection>(cm =>
			{
				cm.AutoMap();
				cm.UnmapProperty(dc => dc.ID);
				cm.UnmapProperty(dc => dc.EnclosingItem);
			});
			BsonClassMap.RegisterClassMap<Relation<ContentItem>>(cm =>
				{
					cm.MapProperty(r => r.ID);
				});
			BsonClassMap.RegisterClassMap<ContentVersion>(cm =>
				{
					cm.AutoMap();
					cm.MapIdProperty(cv => cv.ID).SetIdGenerator(new IntIdGenerator());
					cm.UnmapProperty(cv => cv.Version); // TODO, try to avoid
				});
			BsonClassMap.RegisterClassMap<ContentItem>(cm =>
			{
				cm.AutoMap();
				cm.MapIdProperty(ci => ci.ID).SetIdGenerator(new IntIdGenerator());
				cm.UnmapProperty(ci => ci.Children);
				cm.UnmapProperty(ci => ci.DetailCollections);
				cm.UnmapProperty(ci => ci.Parent);
				cm.UnmapProperty(ci => ci.VersionOf);
				cm.SetIsRootClass(isRootClass: true);
			});

			var definitions = definitionProviders.SelectMany(dp => dp.GetDefinitions()).ToList();
			foreach (var definition in definitions)
			{
				var factory = (ContentClassMapFactory)Activator.CreateInstance(typeof(ContentClassMapFactory<>).MakeGenericType(definition.ItemType));
				BsonClassMap.RegisterClassMap(factory.Create(definition, definitions, activator, this));
			}
		}

		public MongoCollection<T> GetCollection<T>(string collectionName = null)
		{
			return Database.GetCollection<T>(typeof(T).Name);
		}
	}
}
