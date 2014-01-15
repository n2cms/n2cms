using System.Configuration;

namespace N2.Configuration
{
    public class InterfacePluginElement : NamedElement
    {
        [ConfigurationProperty("type", IsRequired = true)]
        public string Type
        {
            get { return (string)base["type"]; }
            set { base["type"] = value; }
        }
    }
}
