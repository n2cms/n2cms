using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using N2.Configuration;
using N2.Definitions;
using N2.Details;
using N2.Engine;
using N2.Security;
using N2.Web;
using NHibernate;
using NHibernate.AdoNet;
using NHibernate.Driver;
using NHibernate.Mapping;
using NHibernate.Mapping.ByCode;
using Environment = NHibernate.Cfg.Environment;

namespace N2.Persistence.NH
{
	/// <summary>
	/// Builds NHibernate configuration by reading hbm files and generating 
	/// mappings for item types without hbm.xml mappings files.
	/// </summary>
	[Service]
	public class ConfigurationBuilder : IConfigurationBuilder
	{
		public const int BlobLength = 2147483647;

		private readonly Engine.Logger<ConfigurationBuilder> logger;
		private readonly ClassMappingGenerator generator;
		private readonly IDefinitionProvider[] definitionProviders;
		private readonly IWebContext webContext;
		private readonly ConfigurationBuilderParticipator[] participators;
	
		IDictionary<string, string> properties = new Dictionary<string, string>();		
		IList<Assembly> assemblies = new List<Assembly>();
		IList<string> mappingNames = new List<string>();
		string tablePrefix = "n2";
		int? batchSize = 25;
		CollectionLazy childrenLaziness = CollectionLazy.Extra;
		Cascade childrenCascade = Cascade.None;
		int stringLength = 1073741823;
		bool tryLocatingHbmResources = false;
		private string cacheRegion;

		/// <summary>Creates a new instance of the <see cref="ConfigurationBuilder"/>.</summary>
		public ConfigurationBuilder(IDefinitionProvider[] definitionProviders, ClassMappingGenerator generator, IWebContext webContext, ConfigurationBuilderParticipator[] participators, DatabaseSection config, ConnectionStringsSection connectionStrings)
		{
			this.definitionProviders = definitionProviders;
			this.generator = generator;
			this.webContext = webContext;
			this.participators = participators;

			if (config == null) config = new DatabaseSection();
			TryLocatingHbmResources = config.TryLocatingHbmResources;
			tablePrefix = config.TablePrefix;
			batchSize = config.BatchSize;
			childrenLaziness = config.Children.Laziness;
			childrenCascade = config.Children.Cascade;
			cacheRegion = config.CacheRegion;

			SetupProperties(config, connectionStrings);
			SetupMappings(config);
		}

		private void SetupMappings(DatabaseSection config)
		{
			foreach (MappingElement me in config.Mappings)
			{
				mappingNames.Add(me.Name);
			}
		}

		/// <summary>Sets properties configuration dictionary based on configuration in the database section.</summary>
		/// <param name="config">The database section configuration.</param>
		/// <param name="connectionStrings">Connection strings from configuration</param>
		protected void SetupProperties(DatabaseSection config, ConnectionStringsSection connectionStrings)
		{
			NHibernate.Cfg.Environment.UseReflectionOptimizer = Utility.GetTrustLevel() > System.Web.AspNetHostingPermissionLevel.Medium;

			// connection

			Properties[NHibernate.Cfg.Environment.ConnectionStringName] = config.ConnectionStringName;
			Properties[NHibernate.Cfg.Environment.ConnectionProvider] = "NHibernate.Connection.DriverConnectionProvider";
			Properties[NHibernate.Cfg.Environment.Hbm2ddlKeyWords] = "none";

			SetupFlavourProperties(config, connectionStrings);

			bool useNonBatcher = 
				// configured batch size <= 1
				(batchSize.HasValue && batchSize.Value <= 1)
				// medium trust in combination with sql client driver 
				// causes fault: Attempt by method 'NHibernate.AdoNet.SqlClientSqlCommandSet..ctor()' to access method 'System.Data.SqlClient.SqlCommandSet..ctor()' failed.   at System.RuntimeTypeHandle.CreateInstance(RuntimeType type, Boolean publicOnly, Boolean noCheck, Boolean& canBeCached, RuntimeMethodHandleInternal& ctor, Boolean& bNeedSecurityCheck)
				|| (Utility.GetTrustLevel() <= System.Web.AspNetHostingPermissionLevel.Medium && typeof(SqlClientDriver).IsAssignableFrom(Type.GetType(Properties[Environment.ConnectionDriver])));
			if (useNonBatcher)
				Properties[NHibernate.Cfg.Environment.BatchStrategy] = typeof(NonBatchingBatcherFactory).AssemblyQualifiedName;

			SetupCacheProperties(config);

			if (config.Isolation.HasValue)
				Properties[NHibernate.Cfg.Environment.Isolation] = config.Isolation.ToString();

			foreach (string key in config.HibernateProperties.AllKeys)
			{
				Properties[key] = config.HibernateProperties[key].Value;
			}
		}

		private DatabaseFlavour SetupFlavourProperties(DatabaseSection config, ConnectionStringsSection connectionStrings)
		{
			DatabaseFlavour flavour = config.Flavour;
			if (flavour == DatabaseFlavour.AutoDetect)
			{
				ConnectionStringSettings css = connectionStrings.ConnectionStrings[config.ConnectionStringName];
				if (css == null)
					throw new ConfigurationErrorsException("Could not find the connection string named '" + config.ConnectionStringName + "' that was defined in the n2/database configuration section. If you installed using NuGet try installing 'N2 CMS SQLite config' or configuring this connection string manually.");
				flavour = DetectFlavor(css);
			}

			// HACK: used to support seamless text/nvarchar(max) support across databases
			if (flavour == DatabaseFlavour.MySql)
				stringLength = 16777215;

			switch (flavour)
			{
				case DatabaseFlavour.SqlServer2000:
					Properties[NHibernate.Cfg.Environment.ConnectionDriver] = typeof(NHibernate.Driver.SqlClientDriver).AssemblyQualifiedName;
					Properties[NHibernate.Cfg.Environment.Dialect] = typeof(NHibernate.Dialect.MsSql2000Dialect).AssemblyQualifiedName;
					break;
				case DatabaseFlavour.SqlServer2005:
					Properties[NHibernate.Cfg.Environment.ConnectionDriver] = typeof(NHibernate.Driver.SqlClientDriver).AssemblyQualifiedName;
					Properties[NHibernate.Cfg.Environment.Dialect] = typeof(NHibernate.Dialect.MsSql2005Dialect).AssemblyQualifiedName;
					break;
				case DatabaseFlavour.SqlServer:
				case DatabaseFlavour.SqlServer2008:
					Properties[NHibernate.Cfg.Environment.ConnectionDriver] = typeof(NHibernate.Driver.SqlClientDriver).AssemblyQualifiedName;
					Properties[NHibernate.Cfg.Environment.Dialect] = typeof(NHibernate.Dialect.MsSql2008Dialect).AssemblyQualifiedName;
					break;
				case DatabaseFlavour.SqlCe3:
					Properties[NHibernate.Cfg.Environment.ConnectionDriver] = typeof(NHibernate.Driver.SqlServerCeDriver).AssemblyQualifiedName;
					Properties[NHibernate.Cfg.Environment.Dialect] = typeof(NHibernate.Dialect.MsSqlCeDialect).AssemblyQualifiedName;
					break;
				case DatabaseFlavour.SqlCe:
				case DatabaseFlavour.SqlCe4:
					Properties[NHibernate.Cfg.Environment.ConnectionDriver] = typeof(NHibernate.Driver.SqlServerCeDriver).AssemblyQualifiedName;
					Properties[NHibernate.Cfg.Environment.Dialect] = typeof(NHibernate.Dialect.MsSqlCe40Dialect).AssemblyQualifiedName;
					break;
				case DatabaseFlavour.MySql:
					Properties[NHibernate.Cfg.Environment.ConnectionDriver] = typeof(NHibernate.Driver.MySqlDataDriver).AssemblyQualifiedName;
					Properties[NHibernate.Cfg.Environment.Dialect] = typeof(NHibernate.Dialect.MySQL5Dialect).AssemblyQualifiedName;
					break;
				case DatabaseFlavour.SqLite:
					Properties[NHibernate.Cfg.Environment.ConnectionDriver] = typeof(NHibernate.Driver.SQLite20Driver).AssemblyQualifiedName;
					Properties[NHibernate.Cfg.Environment.Dialect] = typeof(NHibernate.Dialect.SQLiteDialect).AssemblyQualifiedName;
					break;
				case DatabaseFlavour.Firebird:
					Properties[NHibernate.Cfg.Environment.ConnectionDriver] = typeof(NHibernate.Driver.FirebirdDriver).AssemblyQualifiedName;
					Properties[NHibernate.Cfg.Environment.Dialect] = typeof(NHibernate.Dialect.FirebirdDialect).AssemblyQualifiedName;
					break;
				case DatabaseFlavour.Generic:
					Properties[NHibernate.Cfg.Environment.ConnectionDriver] = typeof(NHibernate.Driver.OleDbDriver).AssemblyQualifiedName;
					Properties[NHibernate.Cfg.Environment.Dialect] = typeof(NHibernate.Dialect.GenericDialect).AssemblyQualifiedName;
					break;
				case DatabaseFlavour.Jet:
					Properties[NHibernate.Cfg.Environment.ConnectionDriver] = "NHibernate.JetDriver.JetDriver, NHibernate.JetDriver";
					Properties[NHibernate.Cfg.Environment.Dialect] = "NHibernate.JetDriver.JetDialect, NHibernate.JetDriver";
					break;
				case DatabaseFlavour.DB2:
					Properties[NHibernate.Cfg.Environment.ConnectionDriver] = typeof(NHibernate.Driver.OdbcDriver).AssemblyQualifiedName;
					Properties[NHibernate.Cfg.Environment.Dialect] = typeof(NHibernate.Dialect.DB2Dialect).AssemblyQualifiedName;
					break;
				case DatabaseFlavour.Oracle9i:
					Properties[NHibernate.Cfg.Environment.ConnectionDriver] = typeof(NHibernate.Driver.OracleClientDriver).AssemblyQualifiedName;
					Properties[NHibernate.Cfg.Environment.Dialect] = typeof(NHibernate.Dialect.Oracle9iDialect).AssemblyQualifiedName;
					break;
				case DatabaseFlavour.Oracle:
				case DatabaseFlavour.Oracle10g:
					// if you have OracleOdpDriver installed
					// use the following line instead of the the later one (NOTICE both apply to the same property)
					// Properties[NHibernate.Cfg.Environment.ConnectionDriver] = typeof(NHibernate.Driver.OracleDataClientDriver).AssemblyQualifiedName;
					Properties[NHibernate.Cfg.Environment.ConnectionDriver] = typeof(NHibernate.Driver.OracleClientDriver).AssemblyQualifiedName;
					Properties[NHibernate.Cfg.Environment.Dialect] = typeof(NHibernate.Dialect.Oracle10gDialect).AssemblyQualifiedName;
					break;
				default:
					throw new ConfigurationErrorsException("Couldn't determine database flavour. Please check the 'flavour' attribute of the n2/database configuration section.");
			}
			return flavour;
		}

		private DatabaseFlavour DetectFlavor(ConnectionStringSettings css)
		{
			string provider = css.ProviderName;

			if (provider == "" || provider.StartsWith("System.Data.SqlClient"))
				return DatabaseFlavour.SqlServer;
			if (provider.StartsWith("System.Data.SQLite"))
				return DatabaseFlavour.SqLite;
			if (provider.StartsWith("MySql.Data.MySqlClient"))
				return DatabaseFlavour.MySql;
			if (provider.StartsWith("System.Data.OracleClient"))
				return DatabaseFlavour.Oracle;
			if (provider.StartsWith("System.Data.SqlServerCe"))
				return DatabaseFlavour.SqlCe;

			throw new ConfigurationErrorsException("Could not auto-detect the database flavor. Please configure this explicitly in the n2/database section.");
		}

		private void SetupCacheProperties(DatabaseSection config)
		{
			Properties[NHibernate.Cfg.Environment.UseSecondLevelCache] = config.Caching.ToString();
			Properties[NHibernate.Cfg.Environment.UseQueryCache] = config.Caching.ToString();
			Properties[NHibernate.Cfg.Environment.CacheProvider] = config.CacheProviderClass;
		}

		#region Properties

		public bool TryLocatingHbmResources
		{
			get { return tryLocatingHbmResources; }
			set { tryLocatingHbmResources = value; }
		}

		/// <summary>Gets assemblies that will be added to the NHibernate configuration.</summary>
		public IList<Assembly> Assemblies
		{
			get { return assemblies; }
			set { assemblies = value; }
		}

		/// <summary>Gets NHibernate configuration properties.</summary>
		public IDictionary<string, string> Properties
		{
			get { return properties; }
			set { properties = value; }
		}

		#endregion

		#region Methods

		/// <summary>Builds a <see cref="NHibernate.Cfg.Configuration"/> by adding properties, default assemblies and generating class mappings for unmapped types.</summary>
		/// <returns></returns>
		public virtual NHibernate.Cfg.Configuration BuildConfiguration()
		{			
			NHibernate.Cfg.Configuration cfg = new NHibernate.Cfg.Configuration();

			AddProperties(cfg);
			AddDefaultMapping(cfg);
			AddMappings(cfg);
			AddAssemblies(cfg);
			GenerateMappings(cfg);
			InvokeParticipators(cfg);

			return cfg;
		}

		private void InvokeParticipators(NHibernate.Cfg.Configuration cfg)
		{
			foreach (var participator in participators)
				participator.AlterConfiguration(cfg);
		}

		protected virtual void AddDefaultMapping(NHibernate.Cfg.Configuration cfg)
		{
			ModelMapper mm = new ModelMapper();

			mm.Class<ContentItem>(ContentItemCustomization);
			mm.Class<ContentDetail>(ContentDetailCustomization);
			mm.Class<DetailCollection>(DetailCollectionCustomization);
			mm.Class<AuthorizedRole>(AuthorizedRoleCustomization);

			var compiledMapping = mm.CompileMappingForAllExplicitlyAddedEntities();
			cfg.AddDeserializedMapping(compiledMapping, "N2");
		}

		void ContentItemCustomization(IClassMapper<ContentItem> ca)
		{
			ca.Table(tablePrefix + "Item");
			ca.Lazy(false);
			ca.Cache(cm => { cm.Usage(CacheUsage.NonstrictReadWrite); cm.Region(cacheRegion); });
			ca.Id(x => x.ID, cm => { cm.Generator(Generators.Native); });
			ca.Discriminator(cm => { cm.Column("Type"); cm.Type(NHibernateUtil.String); });
			ca.Property(x => x.Created, cm => { });
			ca.Property(x => x.Published, cm => { });
			ca.Property(x => x.Updated, cm => { });
			ca.Property(x => x.Expires, cm => { });
			ca.Property(x => x.Name, cm => { cm.Length(250); });
			ca.Property(x => x.ZoneName, cm => { cm.Length(50); });
			ca.Property(x => x.TemplateKey, cm => { cm.Length(50); });
			ca.Property(x => x.TranslationKey, cm => { });
			ca.Property(x => x.Title, cm => { cm.Length(250); });
			ca.Property(x => x.SortOrder, cm => { });
			ca.Property(x => x.Visible, cm => { });
			ca.Property(x => x.SavedBy, cm => { cm.Length(50); });
			ca.Property(x => x.State, cm => { });
			ca.Property(x => x.ChildState, cm => { });
			ca.Property(x => x.AncestralTrail, cm => { cm.Length(100); });
			ca.Property(x => x.VersionIndex, cm => { });
			ca.Property(x => x.AlteredPermissions, cm => { });
			//ca.ManyToOne(x => x.VersionOf, cm => { cm.Column("VersionOfID"); cm.Lazy(LazyRelation.Proxy); cm.Fetch(FetchKind.Select); });
			//ca.Property(x => x.VersionOf, cm =>
			//{
			//    cm.Column("VersionOfID");
			//    cm.Type<ContentRelationFactory>();
			//});
			ca.Component(x => x.VersionOf, cm =>
			{
				cm.Property(cr => cr.ID, pm => pm.Column("VersionOfID"));
			});
			ca.ManyToOne(x => x.Parent, cm => { cm.Column("ParentID"); cm.Lazy(LazyRelation.Proxy); cm.Fetch(FetchKind.Select); });
			ca.Bag(x => x.Children, cm =>
			{
				cm.Key(k => k.Column("ParentID"));
				cm.Inverse(true);
				cm.Type<ContentItemListFactory<ContentItem>>();
				cm.Cascade(childrenCascade);
				cm.OrderBy(ci => ci.SortOrder);
				cm.Lazy(childrenLaziness);
				cm.BatchSize(batchSize ?? 25);
				cm.Cache(m => { m.Usage(CacheUsage.NonstrictReadWrite); m.Region(cacheRegion); });
			}, cr => cr.OneToMany());
			ca.Bag(x => x.Details, cm =>
			{
				cm.Key(k => k.Column("ItemID"));
				cm.Inverse(true);
				cm.Type<ContentListFactory<ContentDetail>>();
				cm.Cascade(Cascade.All | Cascade.DeleteOrphans);
				cm.Fetch(CollectionFetchMode.Select);
				cm.Lazy(CollectionLazy.Lazy);
				cm.Cache(m => { m.Usage(CacheUsage.NonstrictReadWrite); m.Region(cacheRegion); });
				cm.Where("DetailCollectionID IS NULL");
			}, cr => cr.OneToMany());
			ca.Bag(x => x.DetailCollections, cm =>
			{
				cm.Key(k => k.Column("ItemID"));
				cm.Inverse(true);
				cm.Type<ContentListFactory<DetailCollection>>();
				cm.Cascade(Cascade.All | Cascade.DeleteOrphans);
				cm.Fetch(CollectionFetchMode.Select);
				cm.Lazy(CollectionLazy.Lazy);
				cm.Cache(m => { m.Usage(CacheUsage.NonstrictReadWrite); m.Region(cacheRegion); });
			}, cr => cr.OneToMany());
			ca.Bag(x => x.AuthorizedRoles, cm =>
			{
				cm.Key(k => k.Column("ItemID"));
				cm.Inverse(true);
				cm.Cascade(Cascade.All | Cascade.DeleteOrphans);
				cm.Fetch(CollectionFetchMode.Select);
				cm.Lazy(CollectionLazy.Lazy);
				cm.Cache(m => { m.Usage(CacheUsage.NonstrictReadWrite); m.Region(cacheRegion); });
			}, cr => cr.OneToMany());
		}

		void ContentDetailCustomization(IClassMapper<ContentDetail> ca)
		{
			ca.Table(tablePrefix + "Detail");
			ca.Lazy(true);
			ca.Cache(cm => { cm.Usage(CacheUsage.NonstrictReadWrite); cm.Region(cacheRegion); });
			ca.Id(x => x.ID, cm => { cm.Generator(Generators.Native); });
			ca.ManyToOne(x => x.EnclosingItem, cm => { cm.Column("ItemID"); cm.NotNullable(true); cm.Fetch(FetchKind.Select); cm.Lazy(LazyRelation.Proxy); });
			ca.ManyToOne(x => x.EnclosingCollection, cm => { cm.Column("DetailCollectionID"); cm.Fetch(FetchKind.Select); cm.Lazy(LazyRelation.Proxy); });
			ca.Property(x => x.ValueTypeKey, cm => { cm.Column("Type"); cm.Length(10); });
			ca.Property(x => x.Name, cm => { cm.Length(50); });
			ca.Property(x => x.Meta, cm => { cm.Type(NHibernateUtil.StringClob); cm.Length(stringLength); });
			ca.Property(x => x.BoolValue, cm => { });
			ca.Property(x => x.DateTimeValue, cm => { });
			ca.Property(x => x.IntValue, cm => { });
			ca.ManyToOne(x => x.LinkedItem, cm => { cm.Column("LinkValue"); cm.Fetch(FetchKind.Select); cm.Lazy(LazyRelation.Proxy); cm.Cascade(Cascade.None); });
			ca.Property(x => x.DoubleValue, cm => { });
			// if you are using Oracle10g and get 
			// ORA-01461: can bind a LONG value only for insert into a LONG column
			// use the following line instead of the the later one (NOTICE both apply to the same property)
			// ca.Property(x => x.StringValue, cm => { cm.Type(NHibernateUtil.AnsiString); cm.Length(stringLength); });
			ca.Property(x => x.StringValue, cm => { cm.Type(NHibernateUtil.StringClob); cm.Length(stringLength); });
			ca.Property(x => x.ObjectValue, cm => { cm.Column("Value"); cm.Type(NHibernateUtil.Serializable); cm.Length(ConfigurationBuilder.BlobLength); });
		}

		void DetailCollectionCustomization(IClassMapper<DetailCollection> ca)
		{
			ca.Table(tablePrefix + "DetailCollection");
			ca.Lazy(true);
			ca.Cache(cm => { cm.Usage(CacheUsage.NonstrictReadWrite); cm.Region(cacheRegion); });
			ca.Id(x => x.ID, cm => { cm.Generator(Generators.Native); });
			ca.ManyToOne(x => x.EnclosingItem, cm => { cm.Column("ItemID"); cm.Fetch(FetchKind.Select); cm.Lazy(LazyRelation.Proxy); });
			ca.Property(x => x.Name, cm => { cm.Length(50); cm.NotNullable(true); });
			ca.Bag(x => x.Details, cm =>
			{
				cm.Key(k => k.Column("DetailCollectionID"));
				cm.Inverse(true);
				cm.Cascade(Cascade.All | Cascade.DeleteOrphans);
				cm.Lazy(CollectionLazy.Lazy);
				cm.Fetch(CollectionFetchMode.Select);
				cm.Cache(m => { m.Usage(CacheUsage.NonstrictReadWrite); m.Region(cacheRegion); });
			}, cr => cr.OneToMany());
		}

		void AuthorizedRoleCustomization(IClassMapper<AuthorizedRole> ca)
		{
			ca.Table(tablePrefix + "AllowedRole");
			ca.Lazy(false);
			ca.Cache(cm => { cm.Usage(CacheUsage.NonstrictReadWrite); cm.Region(cacheRegion); });
			ca.Id(x => x.ID, cm => { cm.Generator(Generators.Native); });
			ca.ManyToOne(x => x.EnclosingItem, cm => { cm.Column("ItemID"); cm.NotNullable(true); });
			ca.Property(x => x.Role, cm => { cm.Length(50); cm.NotNullable(true); });
		}

		private string FormatMapping(string mappingXml)
		{
			return mappingXml.Replace("{TablePrefix}", tablePrefix)
				.Replace("{StringLength}", stringLength.ToString())
				.Replace("{BatchSize}", batchSize.ToString())
				.Replace("{ChildrenLaziness}", childrenLaziness.ToString().ToLower());
		}

		protected virtual void AddMappings(NHibernate.Cfg.Configuration cfg)
		{
			foreach (string mappingName in this.mappingNames)
			{
				AddMapping(cfg, mappingName);
			}
		}

		/// <summary>Adds mappings to the configuration.</summary>
		/// <param name="cfg">The configuration to add the mappings to.</param>
		/// <param name="name">The resource name of the embedded resource.</param>
		protected virtual void AddMapping(NHibernate.Cfg.Configuration cfg, string name)
		{
			if (!string.IsNullOrEmpty(name))
			{
				using (Stream stream = GetStreamFromName(name))
				{
					if (stream == null) throw new ArgumentException("Could not read stream from embedded resource '" + name + "'", "name");

					using (StreamReader reader = new StreamReader(stream))
					{
						var mappingXml = reader.ReadToEnd();
						mappingXml = FormatMapping(mappingXml);

						var xmlReader = new XmlTextReader(mappingXml, XmlNodeType.Document, null);
						var mappingDocument = cfg.LoadMappingDocument(xmlReader, "N2");
						cfg.AddDeserializedMapping(mappingDocument.Document, mappingDocument.Name);
					}
				}
			}
		}

		protected Stream GetStreamFromName(string name)
		{
			string[] pathAssemblyPair = name.Split(',');
			if (pathAssemblyPair.Length != 2) throw new ArgumentException("Expected the property DefaultMapping to be in the format [manifest resource path],[assembly name] but was: " + name);

			Assembly a = Assembly.Load(pathAssemblyPair[1]);
			return a.GetManifestResourceStream(pathAssemblyPair[0]);
		}

		/// <summary>Generates subclasses nhibernate xml configuration as an alternative to NHibernate definition file and adds them to the configuration.</summary>
		/// <param name="cfg">The nhibernate configuration to build.</param>
		protected virtual void GenerateMappings(NHibernate.Cfg.Configuration cfg)
		{
			var definitions = definitionProviders.SelectMany(dp => dp.GetDefinitions()).ToList();
			var allTypes = definitions
				.Select(d => d.ItemType)
				.SelectMany(t => Utility.GetBaseTypesAndSelf(t))
				.Distinct()
				.Where(t => t.IsSubclassOf(typeof(ContentItem)))
				.Where(t => !IsMapped(cfg, t))
				.OrderBy(t => Utility.InheritanceDepth(t))
				.ToList();

			generator.MapTypes(allTypes, cfg, FormatMapping);
		}

		private bool AddedMappingFromHbmResource(ItemDefinition definition, NHibernate.Cfg.Configuration cfg)
		{
			if (!TryLocatingHbmResources)
				return false;

			Stream hbmXmlStream = definition.ItemType.Assembly.GetManifestResourceStream(definition.ItemType.FullName + ".hbm.xml");
			if (hbmXmlStream == null)
				return false;

			using (hbmXmlStream)
			{
				cfg.AddInputStream(hbmXmlStream);
				return true;
			}
		}

		/// <summary>Adds default assemblies to NHibernate configuration.</summary>
		/// <param name="cfg"></param>
		protected virtual void AddAssemblies(NHibernate.Cfg.Configuration cfg)
		{
			foreach (Assembly a in Assemblies)
				cfg.AddAssembly(a);

			logger.Debug(String.Format("Added {0} assemblies to configuration", Assemblies.Count));
		}

		/// <summary>Adds properties to NHibernate configuration.</summary>
		/// <param name="cfg"></param>
		protected virtual void AddProperties(NHibernate.Cfg.Configuration cfg)
		{
			foreach (KeyValuePair<string, string> pair in Properties)
			{
				cfg.SetProperty(pair.Key, pair.Value);
			}
		}

		/// <summary>Builds a configuration and returns a new <see cref="NHibernate.ISessionFactory"/></summary>
		/// <returns>A new <see cref="NHibernate.ISessionFactory"/>.</returns>
		public ISessionFactory BuildSessionFactory()
		{
			logger.Info("Building Configuration");
			var cfg = BuildConfiguration();
			logger.Info("Building Session Factory");
			var sf = cfg.BuildSessionFactory();
			logger.Info("Built Session Factory");
			return sf;
		}

		/// <summary>Checks whether a type's mapping is added to the NHibernate configuration.</summary>
		/// <param name="cfg">The nhibernate configuration to investigate.</param>
		/// <param name="type">The type to look for</param>
		/// <returns>True if the type is mapped</returns>
		protected virtual bool IsMapped(NHibernate.Cfg.Configuration cfg, Type type)
		{
			foreach (PersistentClass mapping in cfg.ClassMappings)
				if (mapping.MappedClass == type)
					return true;
			return false;
		}

		#endregion
	}
}