using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;
using System.ComponentModel;

namespace N2.Configuration
{
    /// <summary>
    /// Configuration related to integration with ASP.NET and urls.
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

        /// <summary>Look for a content page when the requested resource has no extension.</summary>
        [ConfigurationProperty("observeEmptyExtension")]
        public bool ObserveEmptyExtension
        {
            get { return (bool)base["observeEmptyExtension"] || string.IsNullOrEmpty(Extension); }
            set { base["observeEmptyExtension"] = value; }
        }

        /// <summary>Additional extensions observed by the rewriter.</summary>
        [ConfigurationProperty("observedExtensions"), TypeConverter(typeof(CommaDelimitedStringCollectionConverter))]
        public StringCollection ObservedExtensions
        {
            get { return (CommaDelimitedStringCollection)base["observedExtensions"]; }
            set { base["observedExtensions"] = value; }
        }

        /// <summary>Read the windsor inversion of control container configuration from this configuration section instead of the location configured by <see cref="CastleConfiguration"/>.</summary>
        [ConfigurationProperty("isWeb", DefaultValue = true)]
        public bool IsWeb
        {
            get { return (bool)base["isWeb"]; }
            set { base["isWeb"] = value; }
        }

        /// <summary>Enables rewriting of requests to the page handler of a certain content item.</summary>
        [ConfigurationProperty("rewrite", DefaultValue = RewriteMethod.RewriteRequest)]
        public RewriteMethod Rewrite
        {
            get { return (RewriteMethod)base["rewrite"]; }
            set { base["rewrite"] = value; }
        }
    }
}
