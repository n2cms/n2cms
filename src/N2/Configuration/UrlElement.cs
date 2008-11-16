using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace N2.Configuration
{
	/// <summary>
	/// Configuration related to urls and url parsing.
	/// </summary>
	public class UrlElement : ConfigurationElement
	{
		/// <summary>Cache where appropriate to increase performance..</summary>
		[ConfigurationProperty("enableCaching", DefaultValue = true)]
		public bool EnableCaching
		{
			get { return (bool)base["enableCaching"]; }
			set { base["enableCaching"] = value; }
		}
	}
}
