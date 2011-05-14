using System.Configuration;

namespace N2.Configuration
{
    public class FolderElement : ConfigurationElement
    {
        [ConfigurationProperty("path", IsKey = true)]
        public string Path
        {
            get { return (string)base["path"]; }
            set { base["path"] = value; }
        }
    }
}
