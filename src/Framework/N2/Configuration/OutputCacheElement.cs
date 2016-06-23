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

		[ConfigurationProperty("varyByHeader", DefaultValue = "")]
		public string VaryByHeader
		{
			get { return (string)base["varyByCustom"]; }
			set { base["varyByCustom"] = value; }
		}

		[ConfigurationProperty("varyByCustom", DefaultValue = "")]
		public string VaryByCustom
		{
			get { return (string)base["varyByCustom"]; }
			set { base["varyByCustom"] = value; }
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

		[ConfigurationProperty("invalidateOnChangesTo", DefaultValue = OutputCacheInvalidationMode.Page)]
		public OutputCacheInvalidationMode InvalidateOnChangesTo
		{
			get { return (OutputCacheInvalidationMode)base["invalidateOnChangesTo"]; }
			set { base["invalidateOnChangesTo"] = value; }
		}
	}
}
