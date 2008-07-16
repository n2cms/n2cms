using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace N2.Configuration
{
    public class GlobalizationElement : ConfigurationElement
    {
        [ConfigurationProperty("enabled", DefaultValue = false)]
        public bool Enabled
        {
            get { return (bool)base["enabled"]; }
            set { base["enabled"] = value; }
        }
    }
}
