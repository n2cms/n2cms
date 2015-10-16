using System.Configuration;
using NHibernate.Mapping.ByCode;
using System.Collections.Generic;
using System.Data;
using N2.Engine;

namespace N2.Configuration
{
    /// <summary>
    /// Database configuration section for nhibernate database connection.
    /// </summary>
    public class DatabaseSection : ContentConfigurationSectionBase
    {
		Logger<DatabaseSection> logger;

        /// <summary>Whether cacheing should be enabled.</summary>
        [ConfigurationProperty("caching", DefaultValue = false)]
        public bool Caching
        {
            get { return (bool)base["caching"]; }
            set { base["caching"] = value; }
        }

        /// <summary>Whether cacheing should be enabled.</summary>
        [ConfigurationProperty("tryLocatingHbmResources")]
        public bool TryLocatingHbmResources
        {
            get { return (bool)base["tryLocatingHbmResources"]; }
            set { base["tryLocatingHbmResources"] = value; }
        }

        /// <summary>The nhibernate cache provider class to use.</summary>
        /// <remarks>
        /// Other cache providers:
        /// NHibernate.Cache.NoCacheProvider, NHibernate
        /// NHibernate.Caches.SysCache2.SysCacheProvider,NHibernate.Caches.SysCache2
        /// </remarks>
        [ConfigurationProperty("cacheProviderClass", DefaultValue = "NHibernate.Caches.SysCache2.SysCacheProvider, N2")]
        public string CacheProviderClass
        {
            get { return (string)base["cacheProviderClass"]; }
            set { base["cacheProviderClass"] = value; }
        }

        /// <summary>The cache region name to use.</summary>
        [ConfigurationProperty("cacheRegion", DefaultValue = "N2CMS")]
        public string CacheRegion
        {
            get { return (string)base["cacheRegion"]; }
            set { base["cacheRegion"] = value; }
        }

        /// <summary>The name of the sql dependency to use for cache invalidation.</summary>
        /// <remarks>This setting must be enabled in SQL Server using aspnet_regsql.exe.</remarks>
        [ConfigurationProperty("sqlCacheDependency")]
        public string SqlCacheDependency
        {
            get { return (string)base["sqlCacheDependency"]; }
            set { base["sqlCacheDependency"] = value; }
        }

        /// <summary>The connection string to pick among the connection strings in the connectionStrings section.</summary>
        [ConfigurationProperty("connectionStringName", DefaultValue = "N2CMS")]
        public string ConnectionStringName
        {
            get { return (string)base["connectionStringName"]; }
            set { base["connectionStringName"] = value; }
        }

		/// <summary>The connection string to use.</summary>
		[ConfigurationProperty("connectionString")]
		public string ConnectionString
		{
			get { return (string)base["connectionString"]; }
			set { base["connectionString"] = value; }
		}

        /// <summary>The prefix used for tables in this site. This can be used to install multiple installations in the same database.</summary>
        [ConfigurationProperty("tablePrefix", DefaultValue = "n2")]
        public string TablePrefix
        {
            get { return (string)base["tablePrefix"]; }
            set { base["tablePrefix"] = value; }
        }

        /// <summary>The type of nhibernate laziness to use. Supported values are "true", "false", and "extra".</summary>
        [ConfigurationProperty("children")]
        public ChildrenElement Children
        {
            get { return (ChildrenElement)base["children"]; }
            set { base["children"] = value; }
        }

        /// <summary>NHibernate option for database query batching.</summary>
        [ConfigurationProperty("batchSize")]
        public int? BatchSize
        {
            get { return (int?)base["batchSize"]; }
            set { base["batchSize"] = value; }
        }

        /// <summary>The database flavour decides which propertes the nhibernate configuration will receive.</summary>
        [ConfigurationProperty("flavour", DefaultValue = DatabaseFlavour.AutoDetect)]
        public DatabaseFlavour Flavour
        {
            get { return (DatabaseFlavour)base["flavour"]; }
            set { base["flavour"] = value; }
        }

        /// <summary>The resource name and assembly of the base nhibernate mapping file, e.g. "N2.Mappings.Default.hbm.xml, N2"</summary>
        [ConfigurationProperty("hibernateMapping")]
        public string HibernateMapping
        {
            get { return (string)base["hibernateMapping"]; }
            set { base["hibernateMapping"] = value; }
        }

        /// <summary>Additional nhibernate properties applied after the default flavour-based configuration.</summary>
        [ConfigurationProperty("hibernateProperties")]
        public NameValueConfigurationCollection HibernateProperties
        {
            get { return (NameValueConfigurationCollection)base["hibernateProperties"]; }
            set { base["hibernateProperties"] = value; }
        }

        /// <summary>NHibernate mappings added in addition to the hibernateMapping.</summary>
        [ConfigurationProperty("mappings")]
        public MappingCollection Mappings
        {
            get { return (MappingCollection)this["mappings"]; }
            set { base["mappings"] = value; }
        }

        /// <summary>Search configuration.</summary>
        [ConfigurationProperty("search")]
        public SearchElement Search
        {
            get { return (SearchElement)this["search"]; }
            set { base["search"] = value; }
        }

        /// <summary>Database file system configuration.</summary>
        [ConfigurationProperty("files")]
        public FilesElement Files
        {
            get { return (FilesElement)this["files"]; }
            set { base["files"] = value; }
        }

        /// <summary>Database file system configuration.</summary>
        [ConfigurationProperty("isolation")]
        public IsolationLevel? Isolation
        {
            get { return (IsolationLevel?)this["isolation"]; }
            set { base["isolation"] = value; }
        }

        /// <summary>Starts a transaction every as a connection is opened.</summary>
        [ConfigurationProperty("autoStartTransaction", DefaultValue = false)]
        public bool AutoStartTransaction
        {
            get { return (bool)this["autoStartTransaction"]; }
            set { base["autoStartTransaction"] = value; }
        }

        public override void ApplyComponentConfigurationKeys(List<string> configurationKeys)
        {
			if (!string.IsNullOrEmpty(Search.Type))
				configurationKeys.Add(Search.Type);

			var flavour = Flavour;
			if (flavour == DatabaseFlavour.AutoDetect)
			{
				try
				{
					var cs = ConfigurationManager.ConnectionStrings[ConnectionStringName];
					if (cs != null && cs.ConnectionString != null && cs.ConnectionString.Contains("XmlRepositoryPath="))
						flavour = DatabaseFlavour.Xml;
				}
				catch (System.Exception ex)
				{
					logger.Warn(ex);
				}
			}

            switch (flavour)
            {
                case DatabaseFlavour.MongoDB:
                    configurationKeys.Add("mongo");
                    break;

                case DatabaseFlavour.Xml:
                    configurationKeys.Add("xml");
                    break;
            }

			if ((flavour & DatabaseFlavour.NoSql) == DatabaseFlavour.NoSql)
				configurationKeys.Add("nosql");
			else
				configurationKeys.Add("sql");

            if (Files.StorageLocation == FileStoreLocation.Database)
            {
                if (Flavour == DatabaseFlavour.MongoDB)
                {
                    configurationKeys.Add("mongofs");
                }
                else
                {
                    configurationKeys.Add("dbfs");
                }
            }
        }

    }
}
