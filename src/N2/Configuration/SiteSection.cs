using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace N2.Configuration
{
	public class SiteSection : ConfigurationSection
	{
		[ConfigurationProperty("rootPageID", DefaultValue = 1)]
		public int RootPageID
		{
			get { return (int)base["rootPageID"]; }
			set { base["rootPageID"] = value; }
		}

		[ConfigurationProperty("startPageID", DefaultValue = 1)]
		public int StartPageID
		{
			get { return (int)base["startPageID"]; }
			set { base["startPageID"] = value; }
		}

	}
}
