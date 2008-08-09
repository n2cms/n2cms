using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace N2.Configuration
{
    public class WebElement : ConfigurationElement
    {
        [ConfigurationProperty("extension", DefaultValue = ".aspx")]
        public string Extension
        {
            get { return (string)base["extension"]; }
            set { base["extension"] = value; }
        }

        /// <summary>Read the windsor inversion of control container configuration from this configuration section instead of the location configured by <see cref="CastleConfiguration"/>.</summary>
        [ConfigurationProperty("isWeb", DefaultValue = true)]
        public bool IsWeb
        {
            get { return (bool)base["isWeb"]; }
            set { base["isWeb"] = value; }
        }

        /// <summary>Enables rewriting of requests to the page handler of a certain content item.</summary>
        [ConfigurationProperty("rewriteEnabled", DefaultValue = true)]
        public bool RewriteEnabled
        {
            get { return (bool)base["rewriteEnabled"]; }
            set { base["rewriteEnabled"] = value; }
        }
    }
}
