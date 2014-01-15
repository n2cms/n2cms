using System.Configuration;

namespace N2.Configuration
{
    /// <summary>
    /// Configuration related to managementUrls and url parsing.
    /// </summary>
    public class UrlElement : ConfigurationElement
    {
        /// <summary>Cache where appropriate to increase performance..</summary>
        [ConfigurationProperty("enableCaching", DefaultValue = true)]
        public bool EnableCaching
        {
            get { return (bool)base["enableCaching"]; }
            set { base["enableCaching"] = value; }
        }

        [ConfigurationProperty("nonRewritable")]
        public VirtualPathCollection NonRewritable
        {
            get { return (VirtualPathCollection)base["nonRewritable"]; }
            set { base["nonRewritable"] = value; }
        }
    }
}
