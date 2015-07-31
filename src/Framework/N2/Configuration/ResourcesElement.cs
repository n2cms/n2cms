using System;
using System.ComponentModel;
using System.Configuration;
using N2.Resources;

namespace N2.Configuration
{
	public class ResourcesElement : ConfigurationElement
	{
		/// <summary>Whether to make registered web resources debuggable.</summary>
		[ConfigurationProperty("debug")]
		public bool? Debug
		{
			get { return (bool?)base["debug"]; }
			set { base["debug"] = value; }
		}

		/// <summary>The path to the included jQuery javascript resource.</summary>
		[ConfigurationProperty("jQueryJsPath", DefaultValue = Register.DefaultJQueryJsPath)]
		public string JQueryJsPath
		{
			get { return (string)base["jQueryJsPath"]; }
			set { base["jQueryJsPath"] = value; }
		}

		/// <summary>The path to the included jQuery UI javascript resource.</summary>
		[ConfigurationProperty("jQueryUiPath", DefaultValue = Register.DefaultJQueryUiJsPath)]
		public string JQueryUiPath
		{
			get { return (string)base["jQueryUiPath"]; }
			set { base["jQueryUiPath"] = value; }
		}

		/// <summary>The path to the included jQuery plgins javascript resource.</summary>
		[ConfigurationProperty("jQueryPluginsPath", DefaultValue = "{ManagementUrl}/Resources/Js/plugins.ashx?v=" + Register.JQueryVersion)]
		public string JQueryPluginsPath
		{
			get { return (string)base["jQueryPluginsPath"]; }
			set { base["jQueryPluginsPath"] = value; }
		}

		/// <summary>The path to the included angular javascript resource.</summary>
		[ConfigurationProperty("angularJsRoot", DefaultValue = Register.DefaultAngularJsRoot)]
		public string AngularJsRoot
		{
			get { return (string)base["angularJsRoot"]; }
			set { base["angularJsRoot"] = value; }
		}

		/// <summary>The path to the included angular-strap javascript resource.</summary>
		[ConfigurationProperty("angularStrapJsPath", DefaultValue = Register.DefaultAngularStrapJsRoot)]
		public string AngularStrapJsPath
		{
			get { return (string)base["angularStrapJsPath"]; }
			set { base["angularStrapJsPath"] = value; }
		}

		/// <summary>The path to the included angular-ui javascript resource.</summary>
		[ConfigurationProperty("angularUiJsPath", DefaultValue = Register.DefaultAngularUiJsPath)]
		public string AngularUiJsPath
		{
			get { return (string)base["angularUiJsPath"]; }
			set { base["angularUiJsPath"] = value; }
		}

		/// <summary>The path to the included tiny MCE javascript resource.</summary>
		[ConfigurationProperty("ckEditorJsPath", DefaultValue = Register.DefaultCkEditorPath)]
		public string CkEditorPath
		{
			get { return (string)base["ckEditorJsPath"]; }
			set { base["ckEditorJsPath"] = value; }
		}

		[ConfigurationProperty("fancyboxJsPath", DefaultValue = N2.Resources.Register.DefaultFancyboxJsPath)]
		public string FancyboxJsPath
		{
			get { return (string)base["fancyboxJsPath"]; }
			set { base["fancyboxJsPath"] = value; }
		}

		[ConfigurationProperty("fancyboxCssPath", DefaultValue = N2.Resources.Register.DefaultFancyboxCssPath)]
		public string FancyboxCssPath
		{
			get { return (string)base["fancyboxCssPath"]; }
			set { base["fancyboxCssPath"] = value; }
		}

		[ConfigurationProperty("partsJsPath", DefaultValue = "{ManagementUrl}/Resources/Js/parts.js?v=" + Register.JQueryVersion)]
		public string PartsJsPath
		{
			get { return (string)base["partsJsPath"]; }
			set { base["partsJsPath"] = value; }
		}

		[ConfigurationProperty("partsCssPath", DefaultValue = "{ManagementUrl}/Resources/Css/parts.css?v=" + Register.JQueryVersion)]
		public string PartsCssPath
		{
			get { return (string)base["partsCssPath"]; }
			set { base["partsCssPath"] = value; }
		}

		[ConfigurationProperty("bootstrapJsPath", DefaultValue = Register.DefaultBootstrapJsPath)]
		public string BootstrapJsPath
		{
			get { return (string)base["bootstrapJsPath"]; }
			set { base["bootstrapJsPath"] = value; }
		}

		[ConfigurationProperty("bootstrapCssPath", DefaultValue = Register.DefaultBootstrapCssPath)]
		public string BootstrapCssPath
		{
			get { return (string)base["bootstrapCssPath"]; }
			set { base["bootstrapCssPath"] = value; }
		}

		[ConfigurationProperty("bootstrapVersion", DefaultValue = Register.DefaultBootstrapVersion)]
		[TypeConverter(typeof(Utility.String2Version))]
		public Version BootstrapVersion
		{
			get { return Version.Parse((string)base["bootstrapVersion"]); }
			set { base["bootstrapVersion"] = value.ToString(); }
		}

		[ConfigurationProperty("bootstrapDatePickerJsPath", DefaultValue = Register.DefaultBootstrapDatePickerJsPath)]
		public string BootstrapDatePickerJsPath
		{
			get { return (string)base["bootstrapDatePickerJsPath"]; }
			set { base["bootstrapDatePickerJsPath"] = value; }
		}

		[ConfigurationProperty("bootstrapDatePickerCssPath", DefaultValue = Register.DefaultBootstrapDatePickerCssPath)]
		public string BootstrapDatePickerCssPath
		{
			get { return (string)base["bootstrapDatePickerCssPath"]; }
			set { base["bootstrapDatePickerCssPath"] = value; }
		}

		[ConfigurationProperty("bootstrapTimePickerJsPath", DefaultValue = Register.DefaultBootstrapTimePickerJsPath)]
		public string BootstrapTimePickerJsPath
		{
			get { return (string)base["bootstrapTimePickerJsPath"]; }
			set { base["bootstrapTimePickerJsPath"] = value; }
		}

		[ConfigurationProperty("bootstrapTimePickerCssPath", DefaultValue = Register.DefaultBootstrapTimePickerCssPath)]
		public string BootstrapTimePickerCssPath
		{
			get { return (string)base["bootstrapTimePickerCssPath"]; }
			set { base["bootstrapTimePickerCssPath"] = value; }
		}

		[ConfigurationProperty("iconsCssPath", DefaultValue = N2.Resources.Register.DefaultIconsCssPath)]
		public string IconsCssPath
		{
			get { return (string)base["iconsCssPath"]; }
			set { base["iconsCssPath"] = value; }
		}
	}
}
