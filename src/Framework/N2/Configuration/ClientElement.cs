using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace N2.Configuration
{
    public class ClientElement : ConfigurationElement
    {
        [ConfigurationProperty("url", DefaultValue = "http://localhost:7850/")]
        public string Url
        {
            get { return (string)base["url"]; }
            set { base["url"] = value; }
        }

        [ConfigurationProperty("sharedSecret")]
        public string SharedSecret
        {
            get { return (string)base["sharedSecret"]; }
            set { base["sharedSecret"] = value; }
        }

        [ConfigurationProperty("instanceName", DefaultValue = "Pages")]
        public string InstanceName
        {
            get { return (string)base["instanceName"]; }
            set { base["instanceName"] = value; }
        }

        [ConfigurationProperty("searchTimeout", DefaultValue = 10000)]
        public int SearchTimeout
        {
            get { return (int)base["searchTimeout"]; }
            set { base["searchTimeout"] = value; }
        }

        [ConfigurationProperty("indexTimeout", DefaultValue = 100000)]
        public int IndexTimeout
        {
            get { return (int)base["indexTimeout"]; }
            set { base["indexTimeout"] = value; }
        }
    }
}
