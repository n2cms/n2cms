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
		/// <summary>The granularity of the stored statistics.</summary>
		[ConfigurationProperty("granularity", DefaultValue = Granularity.Day)]
		public Granularity Granularity
		{
			get { return (Granularity)base["granularity"]; }
			set { base["granularity"] = value; }
		}

		/// <summary>How often in-memory buckets are saved to temporary storage.</summary>
		[ConfigurationProperty("memoryFlushInterval", DefaultValue = Granularity.Minute)]
		public Granularity MemoryFlushInterval
		{
			get { return (Granularity)base["memoryFlushInterval"]; }
			set { base["memoryFlushInterval"] = value; }
		}

		/// <summary>How often temporary storage is tranferred into permanent storage.</summary>
		[ConfigurationProperty("transferInterval", DefaultValue = Granularity.Hour)]
		public Granularity TransferInterval
		{
			get { return (Granularity)base["transferInterval"]; }
			set { base["transferInterval"] = value; }
		}

		/// <summary>How many days of data are displayed in the admin info panel.</summary>
		[ConfigurationProperty("displayedDays", DefaultValue = 14)]
		public int DisplayedDays
		{
			get { return (int)base["displayedDays"]; }
			set { base["displayedDays"] = value; }
		}
	}
}