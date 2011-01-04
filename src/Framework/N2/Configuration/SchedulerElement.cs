using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace N2.Configuration
{
    public class SchedulerElement : ConfigurationElement
    {
        [ConfigurationProperty("enabled", DefaultValue = true)]
        public bool Enabled
        {
            get { return (bool)base["enabled"]; }
            set { base["enabled"] = value; }
        }

        [ConfigurationProperty("interval", DefaultValue = 60)]
        public int Interval
        {
            get { return (int)base["interval"]; }
            set { base["interval"] = value; }
        }

        [ConfigurationProperty("keepAlive", DefaultValue = false)]
        public bool KeepAlive
        {
            get { return (bool)base["keepAlive"]; }
            set { base["keepAlive"] = value; }
        }

		[ConfigurationProperty("keepAlivePath", DefaultValue = "{ManagementUrl}/Resources/keepalive/ping.ashx")]
        public string KeepAlivePath
        {
            get { return (string)base["keepAlivePath"]; }
            set { base["keepAlivePath"] = value; }
        }
    }
}
