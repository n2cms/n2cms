using System.Configuration;

namespace N2.Configuration
{
	public class OrganizeElement : ConfigurationElement
	{
		[ConfigurationProperty("useLegacyControlPanel", DefaultValue = false)]
		public bool UseLegacyControlPanel
		{
			get { return (bool)base["useLegacyControlPanel"]; }
			set { base["useLegacyControlPanel"] = value; }
		}
	}
}