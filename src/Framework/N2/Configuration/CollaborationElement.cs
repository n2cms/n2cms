using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace N2.Configuration
{
    public class CollaborationElement : ConfigurationElement
    {
        [ConfigurationProperty("activityTrackingEnabled", DefaultValue = true)]
        public bool ActivityTrackingEnabled
        {
            get { return (bool)base["activityTrackingEnabled"]; }
            set { base["activityTrackingEnabled"] = value; }
        }
    }
}
