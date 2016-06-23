using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace N2.Configuration
{
    public class CollaborationElement : ConfigurationElement
	{
		//[ConfigurationProperty("messagesEnabled", DefaultValue = true)]
		//public bool MessagesEnabled
		//{
		//	get { return (bool)base["messagesEnabled"]; }
		//	set { base["messagesEnabled"] = value; }
		//}

        [ConfigurationProperty("activityTrackingEnabled", DefaultValue = true)]
        public bool ActivityTrackingEnabled
        {
            get { return (bool)base["activityTrackingEnabled"]; }
            set { base["activityTrackingEnabled"] = value; }
        }

		[ConfigurationProperty("pingInterval", DefaultValue = 60)]
		public int PingInterval
		{
			get { return (int)base["pingInterval"]; }
			set { base["pingInterval"] = value; }
		}

		[ConfigurationProperty("pingPath", DefaultValue = "{ManagementUrl}/Collaboration/Ping.ashx")]
		public string PingPath
		{
			get { return (string)base["pingPath"]; }
			set { base["pingPath"] = value; }
		}

		[ConfigurationProperty("errorReportingLevel", DefaultValue = System.Diagnostics.TraceEventType.Error)]
		public System.Diagnostics.TraceEventType ErrorReportingLevel
		{
			get { return (System.Diagnostics.TraceEventType)base["errorReportingLevel"]; }
			set { base["errorReportingLevel"] = value; }
		}
	}
}
