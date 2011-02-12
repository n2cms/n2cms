using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace N2.Configuration
{
	public class SearchElement : ConfigurationElement
	{
		/// <summary>The virtual path to the lucene search index database.</summary>
		[ConfigurationProperty("indexPath", DefaultValue = "~/App_Data/SearchIndex/")]
		public string IndexPath
		{
			get { return (string)base["indexPath"]; }
			set { base["indexPath"] = value; }
		}

		/// <summary>Whether to index in a separate thread.</summary>
		[ConfigurationProperty("asyncIndexing", DefaultValue = true)]
		public bool AsyncIndexing
		{
			get { return (bool)base["asyncIndexing"]; }
			set { base["asyncIndexing"] = value; }
		}

		/// <summary>Enable indexing of content.</summary>
		[ConfigurationProperty("enabled", DefaultValue = true)]
		public bool Enabled
		{
			get { return (bool)base["enabled"]; }
			set { base["enabled"] = value; }
		}
	}
}
