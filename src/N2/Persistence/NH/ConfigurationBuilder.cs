using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Linq;
using N2.Configuration;
using N2.Definitions;
using NHibernate;
using NHibernate.Mapping;
using N2.Engine;

namespace N2.Persistence.NH
{
	/// <summary>
	/// Builds NHibernate configuration by reading hbm files and generating 
	/// mappings for item types without hbm.xml mappings files.
	/// </summary>
	[Service]
	public class ConfigurationBuilder : IConfigurationBuilder
	{
		private ClassMappingGenerator generator;

		bool tryLocatingHbmResources = false;
		readonly IDefinitionManager definitions;
		IDictionary<string, string> properties = new Dictionary<string, string>();
		IList<Assembly> assemblies = new List<Assembly>();
		IList<string> mappingNames = new List<string>();
		string defaultMapping = "N2.Persistence.NH.Mappings.Default.hbm.xml,N2";
		string tablePrefix = "n2";
		int batchSize = 25;
		string childrenLaziness = "extra";
		int stringLength = 1073741823;
		string mappingStartTag = @"<?xml version=""1.0"" encoding=""utf-16""?>
<hibernate-mapping xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns=""urn:nhibernate-mapping-2.2"">";
		string mappingEndTag = "</hibernate-mapping>";


		/// <summary>Creates a new instance of the <see cref="ConfigurationBuilder"/>.</summary>
		public ConfigurationBuilder(IDefinitionManager definitions, ClassMappingGenerator generator, DatabaseSection config, ConnectionStringsSection connectionStrings)
		{
			this.definitions = definitions;
			this.generator = generator;

			if (config == null) config = new DatabaseSection();

			if (!string.IsNullOrEmpty(config.HibernateMapping))
				DefaultMapping = config.HibernateMapping;

			SetupProperties(config, connectionStrings);
			SetupMappings(config);

			TryLocatingHbmResources = config.TryLocatingHbmResources;
			tablePrefix = config.TablePrefix;
			batchSize = config.BatchSize;
			childrenLaziness = config.ChildrenLaziness;
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
			Properties[NHibernate.Cfg.Environment.ProxyFactoryFactoryClass] = "NHibernate.ByteCode.Castle.ProxyFactoryFactory, NHibernate.ByteCode.Castle";

			Properties[NHibernate.Cfg.Environment.ConnectionStringName] = config.ConnectionStringName;
			Properties[NHibernate.Cfg.Environment.ConnectionProvider] = "NHibernate.Connection.DriverConnectionProvider";
			Properties[NHibernate.Cfg.Environment.Hbm2ddlKeyWords] = "none";

			DatabaseFlavour flavour = config.Flavour;
			if (flavour == DatabaseFlavour.AutoDetect)
			{
				ConnectionStringSettings css = connectionStrings.ConnectionStrings[config.ConnectionStringName];
				if (css == null)
					throw new ConfigurationErrorsException("Could not find the connection string named '" + config.ConnectionStringName + "' that was defined in the n2/database configuration section.");
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
				case DatabaseFlavour.SqlServer2008:
					Properties[NHibernate.Cfg.Environment.ConnectionDriver] = typeof(NHibernate.Driver.SqlClientDriver).AssemblyQualifiedName;
					Properties[NHibernate.Cfg.Environment.Dialect] = typeof(NHibernate.Dialect.MsSql2005Dialect).AssemblyQualifiedName;
					break;
				case DatabaseFlavour.SqlCe:
					Properties[NHibernate.Cfg.Environment.ConnectionDriver] = typeof(NHibernate.Driver.SqlServerCeDriver).AssemblyQualifiedName;
					Properties[NHibernate.Cfg.Environment.Dialect] = typeof(NHibernate.Dialect.MsSqlCeDialect).AssemblyQualifiedName;
					break;
				case DatabaseFlavour.MySql:
					Properties[NHibernate.Cfg.Environment.ConnectionDriver] = typeof(NHibernate.Driver.MySqlDataDriver).AssemblyQualifiedName;
					Properties[NHibernate.Cfg.Environment.Dialect] = typeof(NHibernate.Dialect.MySQLDialect).AssemblyQualifiedName;
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
				case DatabaseFlavour.Oracle10g:
					Properties[NHibernate.Cfg.Environment.ConnectionDriver] = typeof(NHibernate.Driver.OracleClientDriver).AssemblyQualifiedName;
					Properties[NHibernate.Cfg.Environment.Dialect] = typeof(NHibernate.Dialect.Oracle10gDialect).AssemblyQualifiedName;
					break;
				default:
					throw new ConfigurationErrorsException("Couldn't determine database flavour. Please check the 'flavour' attribute of the n2/database configuration section.");
			}

			Properties[NHibernate.Cfg.Environment.UseSecondLevelCache] = config.Caching.ToString();
			Properties[NHibernate.Cfg.Environment.UseQueryCache] = config.Caching.ToString();
			Properties[NHibernate.Cfg.Environment.CacheProvider] = config.CacheProviderClass;

			foreach (string key in config.HibernateProperties.AllKeys)
			{
				Properties[key] = config.HibernateProperties[key].Value;
			}
		}

		private DatabaseFlavour DetectFlavor(ConnectionStringSettings css)
		{
			string provider = css.ProviderName;

			if (provider == "" || provider.StartsWith("System.Data.SqlClient"))
				return DatabaseFlavour.SqlServer2005;
			if (provider.StartsWith("System.Data.SQLite"))
				return DatabaseFlavour.SqLite;
			if (provider.StartsWith("MySql.Data.MySqlClient"))
				return DatabaseFlavour.MySql;
			if (provider.StartsWith("System.Data.OracleClient"))
				return DatabaseFlavour.Oracle10g;

			throw new ConfigurationErrorsException("Could not auto-detect the database flavor. Please configure this explicitly in the n2/database section.");
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

		/// <summary>Gets or sets a path to an embedded mapping file that are always added to the NHibernate configuration. Items should be in manifest resource stream format followed by a comma and the assembly name, e.g. "N2.Mappings.MySQL.hbm.xml,N2".</summary>
		public string DefaultMapping
		{
			get { return defaultMapping; }
			set { defaultMapping = value; }
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

			return cfg;
		}

		protected virtual void AddDefaultMapping(NHibernate.Cfg.Configuration cfg)
		{
			using (Stream stream = GetStreamFromName(DefaultMapping))
			using (StreamReader reader = new StreamReader(stream))
			{
				string mappingXml = reader.ReadToEnd();
				mappingXml = FormatMapping(mappingXml);
				cfg.AddXml(mappingXml);
			}
		}

		private string FormatMapping(string mappingXml)
		{
			return mappingXml.Replace("{TablePrefix}", tablePrefix)
				.Replace("{StringLength}", stringLength.ToString())
				.Replace("{BatchSize}", batchSize.ToString())
				.Replace("{ChildrenLaziness}", childrenLaziness);
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
						string mappingXml = reader.ReadToEnd();
						mappingXml = FormatMapping(mappingXml);
						cfg.AddXml(mappingXml);
					}
				}
			}
		}

		protected Stream GetStreamFromName(string name)
		{
			string[] pathAssemblyPair = name.Split(',');
			if (pathAssemblyPair.Length != 2) throw new ArgumentException("Expected the property DefaultMapping to be in the format [manifest resource path],[assembly name] but was: " + DefaultMapping);

			Assembly a = Assembly.Load(pathAssemblyPair[1]);
			return a.GetManifestResourceStream(pathAssemblyPair[0]);
		}

		/// <summary>Generates subclasses nhibernate xml configuration as an alternative to NHibernate definition file and adds them to the configuration.</summary>
		/// <param name="cfg">The nhibernate configuration to build.</param>
		protected virtual void GenerateMappings(NHibernate.Cfg.Configuration cfg)
		{
			Debug.Write("Adding");
			StringBuilder mappings = new StringBuilder(mappingStartTag);

			var allTypes = definitions.GetDefinitions()
				.Select(d => d.ItemType)
				.SelectMany(t => Utility.GetBaseTypesAndSelf(t))
				.Distinct()
				.OrderBy(t => Utility.GetBaseTypes(t).Count())
				.Where(t => t.IsSubclassOf(typeof(ContentItem)))
				.ToList();

			foreach(var type in allTypes)
			{
				string discriminator = type.Name;
				var definition = definitions.GetDefinition(type);
				if(definition != null)
					discriminator = definition.Discriminator ?? discriminator;

				string classMapping = generator.GetMapping(type, type.BaseType, discriminator);
				mappings.Append(classMapping);
			}

			mappings.Append(mappingEndTag);
			cfg.AddXml(FormatMapping(mappings.ToString()));
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
			Debug.WriteLine(String.Format("Added {0} assemblies to configuration", Assemblies.Count));
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
			Debug.WriteLine("Building Session Factory " + DateTime.Now);
			return BuildConfiguration().BuildSessionFactory();
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