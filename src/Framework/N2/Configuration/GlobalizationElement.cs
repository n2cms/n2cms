using System.Configuration;

namespace N2.Configuration
{
    public class GlobalizationElement : ConfigurationElement
    {
        [ConfigurationProperty("enabled", DefaultValue = false)]
        public bool Enabled
        {
            get { return (bool)base["enabled"]; }
            set { base["enabled"] = value; }
        }

		[ConfigurationProperty("autoDeleteTranslations", DefaultValue = false)]
		public bool AutoDeleteTranslations
		{
			get { return (bool)base["autoDeleteTranslations"]; }
			set { base["autoDeleteTranslations"] = value; }
		}
    }
}
