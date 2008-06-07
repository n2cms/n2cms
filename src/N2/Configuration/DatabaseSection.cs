using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace N2.Configuration
{
	public class DatabaseSection : ConfigurationSection
	{
		[ConfigurationProperty("caching", DefaultValue = true)]
		public bool Caching
		{
			get { return (bool)base["caching"]; }
			set { base["caching"] = value; }
		}

		[ConfigurationProperty("cacheProviderClass", DefaultValue = "NHibernate.Caches.SysCache2.SysCacheProvider,NHibernate.Caches.SysCache2")]
		public string CacheProviderClass
		{
			get { return (string)base["cacheProviderClass"]; }
			set { base["cacheProviderClass"] = value; }
		}

		[ConfigurationProperty("connectionStringName", DefaultValue = "N2CMS")]
		public string ConnectionStringName
		{
			get { return (string)base["connectionStringName"]; }
			set { base["connectionStringName"] = value; }
		}

		[ConfigurationProperty("flavour", DefaultValue = DatabaseFlavour.SqlServer2005)]
		public DatabaseFlavour Dialect
		{
			get { return (DatabaseFlavour)base["flavour"]; }
			set { base["flavour"] = value; }
		}
	}
}
