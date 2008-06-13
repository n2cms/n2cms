using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace N2.Configuration
{
	public class SiteElement : ConfigurationElement
	{
		[ConfigurationProperty("id", DefaultValue = 1)]
		public int ID
		{
			get { return (int)base["id"]; }
			set { base["id"] = value; }
		}

		[ConfigurationProperty("name", DefaultValue = "assembly://N2/Configuration/castle.configuration.xml")]
		public string Name
		{
			get { return (string)base["name"]; }
			set { base["name"] = value; }
        }

        /// <summary>Use wildcard matching for this hostname, e.g. both n2cms.com and www.n2cms.com.</summary>
        [ConfigurationProperty("wildcards", DefaultValue = false)]
        public bool Wildcards
        {
            get { return (bool)base["wildcards"]; }
            set { base["wildcards"] = value; }
        }

        /// <summary>Per site settings passed on to the site object.</summary>
        [ConfigurationProperty("settings")]
        public KeyValueConfigurationCollection Settings
        {
            get { return (KeyValueConfigurationCollection)base["settings"]; }
        }
	}
}
