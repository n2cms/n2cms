using System.Configuration;

namespace N2.Configuration
{
    public class DirectUrlInjectorElement : ConfigurationElement
    {
        [ConfigurationProperty("enabled", DefaultValue = true)]
        public bool Enabled
        {
            get { return (bool)base["enabled"]; }
            set { base["enabled"] = value; }
        }
    }
}
