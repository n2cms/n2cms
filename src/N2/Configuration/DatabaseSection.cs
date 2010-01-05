using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace N2.Configuration
{
	/// <summary>
	/// Database configuration section for nhibernate database connection.
	/// </summary>
	public class DatabaseSection : ConfigurationSection
	{
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
		[ConfigurationProperty("cacheProviderClass", DefaultValue = "NHibernate.Cache.NoCacheProvider, NHibernate")]
		public string CacheProviderClass
		{
			get { return (string)base["cacheProviderClass"]; }
			set { base["cacheProviderClass"] = value; }
		}

		/// <summary>The connection string to pick amont the connection strings in the connectionStrings section.</summary>
		[ConfigurationProperty("connectionStringName", DefaultValue = "N2CMS")]
		public string ConnectionStringName
		{
			get { return (string)base["connectionStringName"]; }
			set { base["connectionStringName"] = value; }
		}

		/// <summary>The prefix used for tables in this site. This can be used to install multiple installations in the same database.</summary>
		[ConfigurationProperty("tablePrefix", DefaultValue = "n2")]
		public string TablePrefix
		{
			get { return (string)base["tablePrefix"]; }
			set { base["tablePrefix"] = value; }
		}

		/// <summary>The type of nhibernate laziness to use. Supported values are "true", "false", and "extra".</summary>
		[ConfigurationProperty("childrenLaziness", DefaultValue = "extra")]
		public string ChildrenLaziness
		{
			get { return (string)base["childrenLaziness"]; }
			set { base["childrenLaziness"] = value; }
		}

		/// <summary>The prefix used for tables in this site. This can be used to install multiple installations in the same database.</summary>
		[ConfigurationProperty("batchSize", DefaultValue = "25")]
		public int BatchSize
		{
			get { return (int)base["batchSize"]; }
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
	}
}
