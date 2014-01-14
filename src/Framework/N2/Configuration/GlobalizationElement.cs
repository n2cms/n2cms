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

        [ConfigurationProperty("cache", DefaultValue = false)]
        public bool Cache
        {
            get { return (bool)base["cache"]; }
            set { base["cache"] = value; }
        }

        [ConfigurationProperty("languagesPerSite", DefaultValue = false)]
        public bool LanguagesPerSite
        {
            get { return (bool)base["languagesPerSite"]; }
            set { base["languagesPerSite"] = value; }
        }

        [ConfigurationProperty("autoDeleteTranslations", DefaultValue = false)]
        public bool AutoDeleteTranslations
        {
            get { return (bool)base["autoDeleteTranslations"]; }
            set { base["autoDeleteTranslations"] = value; }
        }
    }
}
