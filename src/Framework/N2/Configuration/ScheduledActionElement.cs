using System;
using System.Configuration;

namespace N2.Configuration
{
	public class ScheduledActionElement :NamedElement
	{
		/// <summary>When not empty this action is only executed on a machine with the same name.</summary>
		[ConfigurationProperty("executeOnMachineNamed")]
		public string ExecuteOnMachineNamed
		{
			get { return (string)base["executeOnMachineNamed"]; }
			set { base["executeOnMachineNamed"] = value; }
		}

		[ConfigurationProperty("repeat")]
		public bool? Repeat
		{
			get { return (bool?)base["repeat"]; }
			set { base["repeat"] = value; }
		}

		[ConfigurationProperty("interval")]
		public TimeSpan? Interval
		{
			get { return (TimeSpan?)base["interval"]; }
			set { base["interval"] = value; }
		}
	}
}
