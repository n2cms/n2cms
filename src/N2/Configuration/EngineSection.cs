using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace N2.Configuration
{
	public class EngineSection : ConfigurationSection
	{
		[ConfigurationProperty("rootPageID", DefaultValue = 1)]
		public int RootPageID
		{
			get { return (int)base["rootPageID"]; }
			set { base["rootPageID"] = value; }
		}

		[ConfigurationProperty("startPageID", DefaultValue = 1)]
		public int StartPageID
		{
			get { return (int)base["startPageID"]; }
			set { base["startPageID"] = value; }
		}

		[ConfigurationProperty("castleConfiguration", DefaultValue = "assembly://N2/Configuration/castle.configuration.xml")]
		public string CastleConfiguration
		{
			get { return (string)base["castleConfiguration"]; }
			set { base["castleConfiguration"] = value; }
		}
	}
}
