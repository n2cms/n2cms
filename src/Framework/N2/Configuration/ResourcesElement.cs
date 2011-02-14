using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace N2.Configuration
{
	public class ResourcesElement : ConfigurationElement
	{
		/// <summary>Whether to make registered web resources debuggable.</summary>
		[ConfigurationProperty("debug", DefaultValue = false)]
		public bool Debug
		{
			get { return (bool)base["debug"]; }
			set { base["debug"] = value; }
		}
	}
}
