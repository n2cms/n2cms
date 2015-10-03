using N2.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace N2.Management.Statistics.Configuration
{
	public class StatisticsSection : ConfigurationSectionBase
	{
		[ConfigurationProperty("granularity", DefaultValue = Granularity.Day)]
		public Granularity Granularity
		{
			get { return (Granularity)base["granularity"]; }
			set { base["granularity"] = value; }
		}

		[ConfigurationProperty("memoryFlushInterval", DefaultValue = Granularity.Minute)]
		public Granularity MemoryFlushInterval
		{
			get { return (Granularity)base["memoryFlushInterval"]; }
			set { base["memoryFlushInterval"] = value; }
		}

		[ConfigurationProperty("displayedDays", DefaultValue = 14)]
		public int DisplayedDays
		{
			get { return (int)base["displayedDays"]; }
			set { base["displayedDays"] = value; }
		}
	}
}