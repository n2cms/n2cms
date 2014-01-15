using System.Configuration;

namespace N2.Configuration
{
    public class PluginInitializerElement : NamedElement
    {
        public PluginInitializerElement()
        {
        }

        public PluginInitializerElement(string name, string type)
        {
            Name = name;
            Type = type;
        }

        [ConfigurationProperty("type", IsRequired = true)]
        public string Type
        {
            get { return (string)base["type"]; }
            set { base["type"] = value; }
        }
    }
}
