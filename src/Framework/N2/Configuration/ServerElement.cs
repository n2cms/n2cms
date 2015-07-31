using System.Configuration;

namespace N2.Configuration
{
    public class ServerElement : ConfigurationElement
    {
        [ConfigurationProperty("urlPrefix", DefaultValue = "http://*:7850/")]
        public string UrlPrefix
        {
            get { return (string)base["urlPrefix"]; }
            set { base["urlPrefix"] = value; }
        }

        [ConfigurationProperty("sharedSecret")]
        public string SharedSecret
        {
            get { return (string)base["sharedSecret"]; }
            set { base["sharedSecret"] = value; }
        }
    }
}
