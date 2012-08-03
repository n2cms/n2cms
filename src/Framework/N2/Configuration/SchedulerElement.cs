using System.Configuration;

namespace N2.Configuration
{
    public class SchedulerElement : LazyRemovableCollection<ScheduledActionElement>
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

		/// <summary>When not empty this prevents the scheduler from running except when the machien name matches the given string.</summary>
		[ConfigurationProperty("executeOnMachineNamed")]
		public string ExecuteOnMachineNamed
		{
			get { return (string)base["executeOnMachineNamed"]; }
			set { base["executeOnMachineNamed"] = value; }
		}
    }
}
