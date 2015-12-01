using System.Configuration;

namespace N2.Configuration
{
	public class OrganizeElement : ConfigurationElement
	{
		[ConfigurationProperty("legacyEnabled", DefaultValue = false)]
		public bool LegacyEnabled
		{
			get { return (bool)base["legacyEnabled"]; }
			set { base["legacyEnabled"] = value; }
		}
	}
}