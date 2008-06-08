using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace N2.Configuration
{
	public class EngineSection : ConfigurationSection
	{
		[ConfigurationProperty("castleConfiguration", DefaultValue = "assembly://N2/Configuration/castle.configuration.xml")]
		public string CastleConfiguration
		{
			get { return (string)base["castleConfiguration"]; }
			set { base["castleConfiguration"] = value; }
		}
	}
}
