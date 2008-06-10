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
		[ConfigurationProperty("caching", DefaultValue = true)]
		public bool Caching
		{
			get { return (bool)base["caching"]; }
			set { base["caching"] = value; }
		}

        /// <summary>The nhibernate cache provider class to use.</summary>
        /// <remarks>
        /// Other cache providers:
        /// NHibernate.Cache.NoCacheProvider,NHibernate
        /// NHibernate.Cache.HashtableCacheProvider,NHibernate
        /// </remarks>
		[ConfigurationProperty("cacheProviderClass", DefaultValue = "NHibernate.Caches.SysCache2.SysCacheProvider,NHibernate.Caches.SysCache2")]
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

        /// <summary>The database flavour decides which propertes the nhibernate configuration will receive.</summary>
		[ConfigurationProperty("flavour", DefaultValue = DatabaseFlavour.SqlServer2005)]
		public DatabaseFlavour Flavour
		{
			get { return (DatabaseFlavour)base["flavour"]; }
			set { base["flavour"] = value; }
		}

        /// <summary>Additional nhibernate properties applied after the default flavour-based configuration.</summary>
        [ConfigurationProperty("properties")]
        public NameValueConfigurationCollection Properties
        {
            get { return (NameValueConfigurationCollection)base["properties"]; }
        }
	}
}
