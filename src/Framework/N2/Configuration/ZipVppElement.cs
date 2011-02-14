using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace N2.Configuration
{
	public class ZipVppElement : NamedElement
	{
		/// <summary>The virtual location of the zip file.</summary>
		[ConfigurationProperty("virtualPath", DefaultValue = ".aspx")]
		public string VirtualPath
		{
			get { return (string)base["virtualPath"]; }
			set { base["virtualPath"] = value; }
		}
	}
}
