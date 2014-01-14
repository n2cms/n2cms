using System;
using System.Configuration;

namespace N2.Configuration
{
    /// <summary>
    /// Configuration related to the free text area included with the CMS.
    /// </summary>
    [Obsolete]
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
		[ConfigurationProperty("cssUrl", DefaultValue = "{ManagementUrl}/Resources/bootstrap/css/boostrap.min.css")]
        public string CssUrl
        {
            get { return (string)base["cssUrl"]; }
            set { base["cssUrl"] = value; }
        }

        /// <summary>Url to the free text area editor configuration script file.</summary>
        [ConfigurationProperty("scriptUrl", DefaultValue = "{ManagementUrl}/Resources/Js/FreeTextArea.js")]
        public string ScriptUrl
        {
            get { return (string)base["scriptUrl"]; }
            set { base["scriptUrl"] = value; }
        }

        /// <summary>Url to the free text area editor configuration script file.</summary>
        [ConfigurationProperty("enableTokenDropdown", DefaultValue = true)]
        public bool EnableTokenDropdown
        {
            get { return (bool)base["enableTokenDropdown"]; }
            set { base["enableTokenDropdown"] = value; }
        }
    }
}
