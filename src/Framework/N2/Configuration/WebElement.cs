using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;

namespace N2.Configuration
{
    /// <summary>
    /// Configuration related to integration with ASP.NET and managementUrls.
    /// </summary>
    public class WebElement : ConfigurationElement
    {
        /// <summary>The default extension used by the url parser.</summary>
        [ConfigurationProperty("extension", DefaultValue = "")]
        public string Extension
        {
            get { return (string)base["extension"]; }
            set { base["extension"] = value; }
        }

        /// <summary>The query string key used to identify the page.</summary>
        [ConfigurationProperty("pageQueryKey", DefaultValue = "n2page")]
        public string PageQueryKey
        {
            get { return (string)base["pageQueryKey"]; }
            set { base["pageQueryKey"] = value; }
        }

        /// <summary>The query string key used to identify the item.</summary>
        [ConfigurationProperty("itemQueryKey", DefaultValue = "n2item")]
        public string ItemQueryKey
        {
            get { return (string)base["itemQueryKey"]; }
            set { base["itemQueryKey"] = value; }
        }

        /// <summary>The query string key used to identify the part.</summary>
        [ConfigurationProperty("partQueryKey", DefaultValue = "n2part")]
        public string PartQueryKey
        {
            get { return (string)base["partQueryKey"]; }
            set { base["partQueryKey"] = value; }
        }

        /// <summary>Key used to access path data from context dictionaries.</summary>
        [ConfigurationProperty("pathDataKey", DefaultValue = "n2path")]
        public string PathDataKey
        {
            get { return (string)base["pathDataKey"]; }
            set { base["pathDataKey"] = value; }
        }

        /// <summary>Look for a content page when the requested resource has no extension.</summary>
        [ConfigurationProperty("observeEmptyExtension", DefaultValue = true)]
        public bool ObserveEmptyExtension
        {
            get { return (bool)base["observeEmptyExtension"]; }
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
            get { return (StringCollection)base["observedExtensions"]; }
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

        /// <summary>Tells the rewriter whether it should rewrite when the url matches an existing file. By default N2 doesn't rewrite when the file exists.</summary>
        [ConfigurationProperty("permissionDeniedHttpCode", DefaultValue = 404)]
        public int PermissionDeniedHttpCode
        {
            get { return (int)base["permissionDeniedHttpCode"]; }
            set { base["permissionDeniedHttpCode"] = value; }
        }
    }
}
