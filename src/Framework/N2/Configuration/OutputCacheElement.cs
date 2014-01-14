using System;
using System.Configuration;

namespace N2.Configuration
{
    public class OutputCacheElement : ConfigurationElement
    {
        [ConfigurationProperty("enabled", DefaultValue = false)]
        public bool Enabled
        {
            get { return (bool)base["enabled"]; }
            set { base["enabled"] = value; }
        }

        [ConfigurationProperty("duration", DefaultValue = 0)]
        public int Duration
        {
            get { return (int)base["duration"]; }
            set { base["duration"] = value; }
        }

        [ConfigurationProperty("varyByParam", DefaultValue = "*")]
        public string VaryByParam
        {
            get { return (string)base["varyByParam"]; }
            set { base["varyByParam"] = value; }
        }

        [ConfigurationProperty("cacheProfile")]
        public string CacheProfile
        {
            get { return (string)base["cacheProfile"]; }
            set { base["cacheProfile"] = value; }
        }

        [ConfigurationProperty("slidingExpiration")]
        public System.TimeSpan? SlidingExpiration
        {
            get { return (TimeSpan?)base["slidingExpiration"]; }
            set { base["slidingExpiration"] = value; }
        }
    }
}
