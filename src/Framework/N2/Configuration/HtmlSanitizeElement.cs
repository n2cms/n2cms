using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace N2.Configuration
{
	public class HtmlSanitizeElement : ConfigurationElement
	{
		/// <summary>
		/// Default to escape HTML in titles and other single-entry fields.
		/// </summary>
		[ConfigurationProperty("encodeHtml", DefaultValue = true)]
		public bool EncodeHtml
		{
			get { return (bool)base["encodeHtml"]; }
			set { base["encodeHtml"] = value; }
		}

	}
}
