using System.Configuration;

namespace N2.Configuration
{
    /// <summary>
    /// Configuration related to the external items feature.
    /// </summary>
    public class ExternalsElement : ConfigurationElement
    {
        /// <summary>A custom external item type to instantiate instead of the default.</summary>
        [ConfigurationProperty("externalItemType", DefaultValue = "N2.Management.Externals.ExternalItem, N2.Management")]
        public string ExternalItemType
        {
            get { return (string)base["externalItemType"]; }
            set { base["externalItemType"] = value; }
        }
    }
}
