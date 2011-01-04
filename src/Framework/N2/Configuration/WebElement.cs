using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;
using System.ComponentModel;

namespace N2.Configuration
{
    /// <summary>
    /// Configuration related to integration with ASP.NET and managementUrls.
    /// </summary>
    public class WebElement : ConfigurationElement
	{
		/// <summary>The default extension used by the url parser.</summary>
		[ConfigurationProperty("extension", DefaultValue = ".aspx")]
		public string Extension
		{
			get { return (string)base["extension"]; }
			set { base["extension"] = value; }
		}

		/// <summary>The query string key used to identify the page.</summary>
		[ConfigurationProperty("pageQueryKey", DefaultValue = "page")]
		public string PageQueryKey
		{
			get { return (string)base["pageQueryKey"]; }
			set { base["pageQueryKey"] = value; }
		}

		/// <summary>The query string key used to identify the item.</summary>
		[ConfigurationProperty("itemQueryKey", DefaultValue = "item")]
		public string ItemQueryKey
		{
			get { return (string)base["itemQueryKey"]; }
			set { base["itemQueryKey"] = value; }
		}

		/// <summary>The query string key used to identify the part.</summary>
		[ConfigurationProperty("partQueryKey", DefaultValue = "part")]
		public string PartQueryKey
		{
			get { return (string)base["partQueryKey"]; }
			set { base["partQueryKey"] = value; }
		}

        /// <summary>Look for a content page when the requested resource has no extension.</summary>
        [ConfigurationProperty("observeEmptyExtension")]
        public bool ObserveEmptyExtension
        {
            get { return (bool)base["observeEmptyExtension"] || string.IsNullOrEmpty(Extension) || Extension == "/"; }
            set { base["observeEmptyExtension"] = value; }
		}

		/// <summary>Look for a content page when the requested resource has an unknown extension.</summary>
		[ConfigurationProperty("observeAllExtensions")]
		public bool ObserveAllExtensions
		{
			get { return (bool)base["observeAllExtensions"] || string.IsNullOrEmpty(Extension) || Extension == "/"; }
			set { base["observeAllExtensions"] = value; }
		}

        /// <summary>Additional extensions observed by the rewriter.</summary>
        [ConfigurationProperty("observedExtensions"), TypeConverter(typeof(CommaDelimitedStringCollectionConverter))]
        public StringCollection ObservedExtensions
        {
            get { return (CommaDelimitedStringCollection)base["observedExtensions"]; }
            set { base["observedExtensions"] = value; }
        }

        /// <summary>Whether the current application is running in a web context. This affects how database sessions are stored during a request.</summary>
        [ConfigurationProperty("isWeb", DefaultValue = true)]
        public bool IsWeb
        {
            get { return (bool)base["isWeb"]; }
            set { base["isWeb"] = value; }
        }

        /// <summary>Enables rewriting of requests to the page handler of a certain content item.</summary>
		[ConfigurationProperty("rewrite", DefaultValue = RewriteMethod.SurroundMapRequestHandler)]
        public RewriteMethod Rewrite
        {
            get { return (RewriteMethod)base["rewrite"]; }
            set { base["rewrite"] = value; }
        }

        /// <summary>Tells the rewriter whether it should rewrite when the url matches an existing file. By default N2 doesn't rewrite when the file exists.</summary>
        [ConfigurationProperty("ignoreExistingFiles", DefaultValue = true)]
        public bool IgnoreExistingFiles
        {
            get { return (bool)base["ignoreExistingFiles"]; }
            set { base["ignoreExistingFiles"] = value; }
        }

		/// <summary>Configuration related to managementUrls and url parsing.</summary>
		[ConfigurationProperty("urls")]
		public UrlElement Urls
		{
			get { return (UrlElement)base["urls"]; }
			set { base["urls"] = value; }
		}
    }
}
