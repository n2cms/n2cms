using System.Configuration;

namespace N2.Configuration
{
    public class VirtualPathElement : NamedElement
    {
        public VirtualPathElement()
        {
        }
        public VirtualPathElement(string name, string virtualpath)
        {
            Name = name;
            VirtualPath = virtualpath;
        }

        [ConfigurationProperty("virtualPath", IsRequired = true)]
        public string VirtualPath
        {
            get { return (string)base["virtualPath"]; }
            set { base["virtualPath"] = value; }
        }
    }
}
