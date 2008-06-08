using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace N2.Configuration
{
	public class SitesSection : ConfigurationElementCollection
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

		protected override ConfigurationElement CreateNewElement()
		{
			return new SiteElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((SiteElement)element).Name;
		}
	}
}
