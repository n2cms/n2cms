using System.Configuration;

namespace N2.Configuration
{
    public class TypeElement : ConfigurationElement
    {
        [ConfigurationProperty("typeName", IsKey = true)]
        public string TypeName
        {
            get { return (string)base["typeName"]; }
            set { base["typeName"] = value; }
        }
    }
}
