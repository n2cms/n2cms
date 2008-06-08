using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace N2.Configuration
{
	public class SiteElement : ConfigurationElement
	{
		[ConfigurationProperty("startPageID", DefaultValue = 1)]
		public int StartPageID
		{
			get { return (int)base["startPageID"]; }
			set { base["startPageID"] = value; }
		}

		[ConfigurationProperty("name", DefaultValue = "assembly://N2/Configuration/castle.configuration.xml")]
		public string Name
		{
			get { return (string)base["name"]; }
			set { base["name"] = value; }
		}
	}
}
