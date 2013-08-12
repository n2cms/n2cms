using System;
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
		[ConfigurationProperty("configJsPath")]
		public string ConfigJsPath
		{
			get { return (string)base["configJsPath"]; }
			set { base["configJsPath"] = value; }
		}
	}
}
