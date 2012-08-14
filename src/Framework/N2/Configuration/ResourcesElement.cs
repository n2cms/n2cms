using System.Configuration;

namespace N2.Configuration
{
	public class ResourcesElement : ConfigurationElement
	{
		public const string JQueryVersion = "1.7.2";

		/// <summary>Whether to make registered web resources debuggable.</summary>
		[ConfigurationProperty("debug", DefaultValue = false)]
		public bool Debug
		{
			get { return (bool)base["debug"]; }
			set { base["debug"] = value; }
		}

		/// <summary>The path to the included jQuery javascript resource.</summary>
		[ConfigurationProperty("jQueryPath", DefaultValue = "{ManagementUrl}/Resources/Js/jquery-" + JQueryVersion + ".min.js")]
		public string JQueryPath
		{
			get { return (string)base["jQueryPath"]; }
			set { base["jQueryPath"] = value; }
		}

		/// <summary>The path to the included jQuery UI javascript resource.</summary>
		[ConfigurationProperty("jQueryUiPath", DefaultValue = "{ManagementUrl}/Resources/Js/jquery.ui.ashx?v=" + JQueryVersion)]
		public string JQueryUiPath
		{
			get { return (string)base["jQueryUiPath"]; }
			set { base["jQueryUiPath"] = value; }
		}

		/// <summary>The path to the included jQuery plgins javascript resource.</summary>
		[ConfigurationProperty("jQueryPluginsPath", DefaultValue = "{ManagementUrl}/Resources/Js/plugins.ashx?v=" + JQueryVersion)]
		public string JQueryPluginsPath
		{
			get { return (string)base["jQueryPluginsPath"]; }
			set { base["jQueryPluginsPath"] = value; }
		}

		/// <summary>The path to the included tiny MCE javascript resource.</summary>
		[ConfigurationProperty("tinyMCEPath", DefaultValue = "{ManagementUrl}/Resources/tiny_mce/tiny_mce.js?v=" + JQueryVersion)]
		public string TinyMCEPath
		{
			get { return (string)base["tinyMCEPath"]; }
			set { base["tinyMCEPath"] = value; }
		}

		[ConfigurationProperty("partsJsPath", DefaultValue = "{ManagementUrl}/Resources/Js/parts.js?v=" + JQueryVersion)]
		public string PartsJsPath
		{
			get { return (string)base["partsJsPath"]; }
			set { base["partsJsPath"] = value; }
		}

		[ConfigurationProperty("partsCssPath", DefaultValue = "{ManagementUrl}/Resources/Css/parts.css?v=" + JQueryVersion)]
		public string PartsCssPath
		{
			get { return (string)base["partsCssPath"]; }
			set { base["partsCssPath"] = value; }
		}
	}
}
