using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace N2.Configuration
{
	public class HostSection : ConfigurationSection
	{
		[ConfigurationProperty("rootID", DefaultValue = 1)]
		public int RootID
		{
			get { return (int)base["rootID"]; }
			set { base["rootID"] = value; }
		}

		[ConfigurationProperty("startPageID", DefaultValue = 1)]
		public int StartPageID
		{
			get { return (int)base["startPageID"]; }
			set { base["startPageID"] = value; }
		}

		/// <summary>Enable multiple sites functionality.</summary>
		[ConfigurationProperty("multipleSites", DefaultValue = false)]
		public bool MultipleSites
		{
			get { return (bool)base["multipleSites"]; }
			set { base["multipleSites"] = value; }
		}

		/// <summary>Examine content nodes to find items that are site providers.</summary>
		[ConfigurationProperty("dynamicSites", DefaultValue = true)]
		public bool DynamicSites
		{
			get { return (bool)base["dynamicSites"]; }
			set { base["dynamicSites"] = value; }
		}

		[ConfigurationProperty("sites")]
		public SiteElementCollection Sites
		{
			get { return (SiteElementCollection)base["sites"]; }
		}
	}
}
