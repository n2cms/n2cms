﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace N2.Configuration
{
	/// <summary>
	/// Represents the configuration entity for the CKEditor WYSIWYG HTML editor.
	/// </summary>
	public class CkEditorElement : ConfigurationElement
	{
		[ConfigurationProperty("ckConfigJsPath")]
		public string ConfigJsPath
		{
			get { return (string)base["ckConfigJsPath"]; }
			set { base["ckConfigJsPath"] = value; }
		}

		[ConfigurationProperty("overwriteStylesSet")]
		public string OverwriteStylesSet
		{
			get { return (string)base["overwriteStylesSet"]; }
			set { base["overwriteStylesSet"] = value; }
		}

		[ConfigurationProperty("overwriteFormatTags")]
		public string OverwriteFormatTags
		{
			get { return (string)base["overwriteFormatTags"]; }
			set { base["overwriteFormatTags"] = value; }
		}

		[ConfigurationProperty("contentsCssPath")]
		public string ContentsCssPath
		{
			get { return (string)base["contentsCssPath"]; }
			set { base["contentsCssPath"] = value; }
		}

		[ConfigurationProperty("advancedMenus")]
		public bool AdvancedMenus
		{
			get { return (bool)base["advancedMenus"]; }
			set { base["advancedMenus"] = value; }
		}

		[ConfigurationProperty("overwriteLanguage")]
		public string OverwriteLanguage
		{
			get { return (string)base["overwriteLanguage"]; }
			set { base["overwriteLanguage"] = value; }
		}
	}
}
