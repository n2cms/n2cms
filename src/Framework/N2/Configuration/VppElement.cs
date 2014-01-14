using System.Configuration;

namespace N2.Configuration
{
    public class VppElement : ConfigurationElement
    {
        /// <summary>Zip files to register as virtual path providers.</summary>
        [ConfigurationProperty("zips")]
        public ZipVppCollection Zips
        {
            get { return (ZipVppCollection)base["zips"]; }
            set { base["zips"] = value; }
        }
    }
}
