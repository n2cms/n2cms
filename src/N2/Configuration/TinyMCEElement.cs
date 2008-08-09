using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace N2.Configuration
{
    /// <summary>
    /// Configuration related to the free text area included with the CMS.
    /// </summary>
    public class TinyMCEElement : ConfigurationElement
    {
        /// <summary>Set enabled to false to disable the tinyMCE free text editor.</summary>
        [ConfigurationProperty("enabled", DefaultValue = true)]
        public bool Enabled
        {
            get { return (bool)base["enabled"]; }
            set { base["enabled"] = value; }
        }

        /// <summary>TinyMCE configuration settings. Any configuration setting defined here will take precedence over the default configuration. Please visit http://wiki.moxiecode.com/index.php/TinyMCE:Configuration for configuration options.</summary>
        [ConfigurationProperty("settings")]
        public KeyValueConfigurationCollection Settings
        {
            get { return (KeyValueConfigurationCollection)base["settings"]; }
            set { base["settings"] = value; }
        }

        /// <summary>Url to the free text area editor css file.</summary>
        [ConfigurationProperty("cssUrl", DefaultValue = "~/Edit/Css/Editor.css")]
        public string CssUrl
        {
            get { return (string)base["cssUrl"]; }
            set { base["cssUrl"] = value; }
        }
    }
}
