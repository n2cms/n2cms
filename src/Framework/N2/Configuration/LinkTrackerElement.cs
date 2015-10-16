using System.Configuration;

namespace N2.Configuration
{
	public class LinkTrackerElement : ConfigurationElement
	{
		[ConfigurationProperty("enabled", DefaultValue = true)]
		public bool Enabled
		{
			get { return (bool)base["enabled"]; }
			set { base["enabled"] = value; }
		}

		[ConfigurationProperty("permanentRedirectEnabled", DefaultValue = true)]
		public bool PermanentRedirectEnabled
		{
			get { return (bool)base["permanentRedirectEnabled"]; }
			set { base["permanentRedirectEnabled"] = value; }
		}
	}
}
