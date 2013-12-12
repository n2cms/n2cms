using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace N2.Configuration
{
    public class TargetingCollection : LazyRemovableCollection<TargetingElement>
    {
        [ConfigurationProperty("enabled", DefaultValue = true)]
        public bool Enabled
        {
            get { return (bool)base["enabled"]; }
            set { base["enabled"] = value; }
        }
    }
}
