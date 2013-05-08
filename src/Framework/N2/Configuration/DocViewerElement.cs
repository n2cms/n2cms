using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Configuration;

namespace N2.Configuration
{
	/// <summary>
	/// Configuration related to document viewer extension.
	/// </summary>
	public class DocViewerElement : ConfigurationElement
	{

		[ConfigurationProperty("enabled", DefaultValue = true)]
		public bool Enabled
		{
			get { return (bool)base["enabled"]; }
			set { base["enabled"] = value; }
		}


		/// <summary>
		/// The URL to the document viewer. Put {abs} and {rel} for the absolute and/or relative URL of the document to be viewed. 
		/// For URL Escaping, put {abs_encoded} and {rel_encoded} instead. 
		/// </summary>
		[ConfigurationProperty("url", DefaultValue = "http://docs.google.com/viewer?url={abs}")]
		public string Url
		{
			get { return (string)base["url"]; }
			set { base["url"] = value; }
		}

		/// <summary>
		/// The maximum size of any document to be loaded by the Document Viewer.
		/// </summary>
		[ConfigurationProperty("maxsize", DefaultValue = "")]
		public string MaxSize
		{
			get { return (string)base["maxsize"]; }
			set { base["maxsize"] = value; }
		}

		/// <summary>
		/// The file extensions that the Document Viewer will handle.
		/// </summary>
		[ConfigurationProperty("extensions", DefaultValue = ".TIFF,.CSS,.HTML,.PHP,.C,.CPP,.H,.HPP,.JS,.DOC,.DOCX,.XLS,.XLSX,.PPT,.PPTX,.PDF,.PAGES,.AI,.PSD,.TIFF,.DXF,.SVG,.EPS,.PS,.TTF,.XPS")]
		public string FileExtensions
		{
			get { return (string)base["extensions"]; }
			set { base["extensions"] = value; }
		}

		public string[] FileExtensionsArray
		{
			get { return FileExtensions.Split(','); }
		}

	}
}
