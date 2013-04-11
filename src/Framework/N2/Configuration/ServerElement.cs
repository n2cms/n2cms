using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace N2.Configuration
{
	public class ServerElement : ConfigurationElement
	{
		[ConfigurationProperty("url", DefaultValue = "http://*:7850/")]
		public string Url
		{
			get { return (string)base["url"]; }
			set { base["url"] = value; }
		}
	}
}
