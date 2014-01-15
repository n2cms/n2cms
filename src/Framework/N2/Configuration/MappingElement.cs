using System.Configuration;

namespace N2.Configuration
{
    /// <summary>
    /// References a nhibernate mapping file.
    /// </summary>
    public class MappingElement : ConfigurationElement
    {
        /// <summary>The resource name and assembly of the base nhibernate mapping file, e.g. "N2.Mappings.Default.hbm.xml, N2"</summary>
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)base["name"]; }
            set { base["name"] = value; }
        }
    }
}
