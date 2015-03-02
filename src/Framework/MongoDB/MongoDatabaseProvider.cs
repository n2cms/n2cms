using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Driver;
using N2.Definitions;
using N2.Details;
using N2.Edit.Versioning;
using N2.Engine;
using N2.Persistence.Proxying;
using N2.Security;
using N2.Web;
using System;
using System.Collections.Generic;
using System.Linq;

namespace N2.Persistence.MongoDB
{
    public class MongoIdentityMap
    {
        Dictionary<string, object> map = new Dictionary<string, object>();
        HashSet<string> keyStack = new HashSet<string>();

        public void Set(string key, object entity)
        {
            map[key] = entity;
        }

        public object Get(string key)
        {
            object entity;
            if (!map.TryGetValue(key, out entity))
                return null;
            return entity;
        }

        public TEntity GetOrCreate<TKey, TEntity>(TKey id, Func<TKey, TEntity> factory)
        {
            object entity;
            string key = GetKey<TEntity>(id);
            if (!map.TryGetValue(key, out entity))
            {
                if (keyStack.Contains(key))
                    // stack overflow protection
                    return default(TEntity);

                try
                {
                    keyStack.Add(key);
                    entity = factory(id);
                    Set(key, entity);
                }
                finally
                {
                    keyStack.Remove(key);
                }
            }
            return (TEntity)entity;
        }

        public static string GetKey<TEntity>(object id)
        {
            return typeof(TEntity).Name + "#" + id;
        }

        internal void Remove<TEntity>(object id)
        {
            string key = GetKey<TEntity>(id);
            if (map.ContainsKey(GetKey<TEntity>(id)))
                map.Remove(key);
        }

        public object Current { get; set; }
    }

    [Service]
    public class MongoDatabaseProvider : IDisposable
    {
        private IWebContext webContext;
        Logger<MongoDatabaseProvider> logger;
        static bool isRegistered = false;
        private IServiceContainer services;

        public MongoDatabaseProvider(IServiceContainer services, IProxyFactory proxies, Configuration.ConfigurationManagerWrapper config, IDefinitionProvider[] definitionProviders, IWebContext webContext)
        {
            this.services = services;
            this.webContext = webContext;
            if (config.Sections.Database.Flavour == Configuration.DatabaseFlavour.MongoDB)
            {
                Register(definitionProviders, proxies);
                Connect(config);
            }
        }

        public MongoIdentityMap IdentityMap
        {
            get
            {
                var map = webContext.RequestItems["MongoIdentityMap"] as MongoIdentityMap;
                if (map == null)
                    webContext.RequestItems["MongoIdentityMap"] = map = new MongoIdentityMap();
                return map;
            }
            set
            {
                webContext.RequestItems["MongoIdentityMap"] = value;
            }
        }

        public MongoDatabase Database { get; set; }

        private void Connect(Configuration.ConfigurationManagerWrapper config)
        {
            var connectionString = config.GetConnectionString();
            var settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
            settings.ConnectTimeout = TimeSpan.FromSeconds(10);
            var client = new MongoClient(settings);
            var server = client.GetServer();
            var databaseSettings = new MongoDatabaseSettings()
            {
                ReadPreference = ReadPreference.Nearest,
                WriteConcern = WriteConcern.Acknowledged,
            };
            Database = server.GetDatabase(config.Sections.Database.TablePrefix, databaseSettings);
            try
            {
                GetCollection<ContentItem>().EnsureIndex("Details.Name", "Details.LinkedItem", "Details.StringValue");
                //GetCollection<ContentItem>().EnsureIndex("DetailCollections.Details.Name", "DetailCollections.Details.LinkedItem", "DetailCollections.Details.StringValue");
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
            }
        }

		class ContentConventionPack : IConventionPack
		{
			public IEnumerable<IConvention> Conventions
			{
				get 
				{
					return new IConvention[] 
					{
						new IgnoreIfNullConvention(true),
						new IgnoreUnderscoreMemberFinderConvention()
					};
				}
			}
		}


        private void Register(IDefinitionProvider[] definitionProviders, IProxyFactory proxies)
        {
            if (isRegistered)
                return;
            isRegistered = true;

			//conventions.SetIgnoreIfNullConvention(new AlwaysIgnoreIfNullConvention());
			//conventions.SetMemberFinderConvention(new IgnoreUnderscoreMemberFinderConvention());
            
			ConventionRegistry.Register("ContentConventions", new ContentConventionPack(), t => true);

            BsonSerializer.RegisterSerializationProvider(new ContentSerializationProvider(this, proxies));

            BsonClassMap.RegisterClassMap<AuthorizedRole>(cm =>
            {
                cm.AutoMap();
                cm.UnmapProperty(cd => cd.ID);
                cm.UnmapField(cd => cd.EnclosingItem);
                cm.SetIgnoreExtraElements(true);
            });
            BsonClassMap.RegisterClassMap<ContentDetail>(cm =>
            {
                cm.AutoMap();
                cm.UnmapProperty(cd => cd.ID);
                cm.UnmapProperty(cd => cd.EnclosingCollection);
                cm.UnmapProperty(cd => cd.EnclosingItem);
                cm.UnmapProperty(cd => cd.Value);
                cm.MapProperty(cd => cd.LinkedItem).SetSerializer(new ContentRelationSerializer(this));
                cm.UnmapProperty(cd => cd.LinkValue);
                cm.GetMemberMap(cd => cd.DateTimeValue).SetSerializationOptions(new DateTimeSerializationOptions(DateTimeKind.Local));
                cm.SetIgnoreExtraElements(true);
            });
            BsonClassMap.RegisterClassMap<DetailCollection>(cm =>
            {
                cm.AutoMap();
                cm.UnmapProperty(dc => dc.ID);
                cm.UnmapProperty(dc => dc.EnclosingItem);
                cm.SetIgnoreExtraElements(true);
            });
            BsonClassMap.RegisterClassMap<Relation<ContentItem>>(cm =>
                {
                    cm.MapProperty(r => r.ID);
                    cm.MapProperty(r => r.ValueAccessor).SetSerializer(new RelationValueAccessorSerializer<ContentItem>(this));
                cm.SetIgnoreExtraElements(true);
                });
            BsonClassMap.RegisterClassMap<ContentRelation>(cm =>
                {
                cm.SetIgnoreExtraElements(true);
                });
            BsonClassMap.RegisterClassMap<ContentVersion>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdProperty(cv => cv.ID).SetIdGenerator(new IntIdGenerator());
                    cm.GetMemberMap(cv => cv.Published).SetSerializationOptions(new DateTimeSerializationOptions(DateTimeKind.Local));
                    cm.GetMemberMap(cv => cv.Saved).SetSerializationOptions(new DateTimeSerializationOptions(DateTimeKind.Local));
                    cm.GetMemberMap(cv => cv.FuturePublish).SetSerializationOptions(new DateTimeSerializationOptions(DateTimeKind.Local));
                    cm.GetMemberMap(cv => cv.Expired).SetSerializationOptions(new DateTimeSerializationOptions(DateTimeKind.Local));
                cm.SetIgnoreExtraElements(true);
                });
            BsonClassMap.RegisterClassMap<ContentItem>(cm =>
            {
                cm.AutoMap();
                cm.MapIdProperty(ci => ci.ID).SetIdGenerator(new IntIdGenerator());
                cm.UnmapProperty(ci => ci.Children);
                cm.GetMemberMap(ci => ci.Parent).SetSerializer(new ContentItemReferenceSerializer(this));
                cm.UnmapProperty(ci => ci.VersionOf);
                cm.GetMemberMap(ci => ci.Created).SetSerializationOptions(new DateTimeSerializationOptions(DateTimeKind.Local));
                cm.GetMemberMap(ci => ci.Updated).SetSerializationOptions(new DateTimeSerializationOptions(DateTimeKind.Local));
                cm.GetMemberMap(ci => ci.Published).SetSerializationOptions(new DateTimeSerializationOptions(DateTimeKind.Local));
                cm.GetMemberMap(ci => ci.Expires).SetSerializationOptions(new DateTimeSerializationOptions(DateTimeKind.Local));
                cm.SetIsRootClass(isRootClass: true);
                cm.SetIgnoreExtraElements(true);
            });

            var definitions = definitionProviders.SelectMany(dp => dp.GetDefinitions()).ToList();
            foreach (var definition in definitions)
            {
                var factoryType = typeof(ContentClassMapFactory<>).MakeGenericType(definition.ItemType);
                var factory = (ContentClassMapFactory)Activator.CreateInstance(factoryType, proxies, this, services);
                BsonClassMap.RegisterClassMap(factory.Create(definition, definitions));
            }
        }

        public MongoCollection<T> GetCollection<T>(string collectionName = null)
        {
            return Database.GetCollection<T>(typeof(T).Name);
        }

        public void Dispose()
        {
            IdentityMap = null;
        }

        public void DropDatabases()
        {
            foreach (var collectionName in Database.GetCollectionNames().Where(cn => !cn.StartsWith("system.")))
                Database.GetCollection(collectionName).RemoveAll();
            Dispose();
        }
    }
}
