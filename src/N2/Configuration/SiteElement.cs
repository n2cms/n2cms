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
	}
}
