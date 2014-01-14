using System.Configuration;
using N2.Edit;

namespace N2.Configuration
{
    /// <summary>
    /// Configuration related to versioning.
    /// </summary>
    public class VersionsElement : ConfigurationElement
    {
        /// <summary>Whether versions are stored when saving items using the editor interface.</summary>
        [ConfigurationProperty("enabled", DefaultValue = true)]
        public bool Enabled
        {
            get { return (bool)base["enabled"]; }
            set { base["enabled"] = value; }
        }

        /// <summary>Max versions to store for each item.</summary>
        [ConfigurationProperty("maximumPerItem", DefaultValue = 100)]
        public int MaximumPerItem
        {
            get { return (int)base["maximumPerItem"]; }
            set { base["maximumPerItem"] = value; }
        }

        /// <summary>Whether versions are stored when saving items using the editor interface.</summary>
        [ConfigurationProperty("defaultViewMode", DefaultValue = ViewPreference.Draft)]
        public ViewPreference DefaultViewMode
        {
            get { return (ViewPreference)base["defaultViewMode"]; }
            set { base["defaultViewMode"] = value; }
        }

        /// <summary>Show recent versions while editing.</summary>
        [ConfigurationProperty("showRecentVersions", DefaultValue = true)]
        public bool ShowRecentVersions
        {
            get { return (bool)base["showRecentVersions"]; }
            set { base["showRecentVersions"] = value; }
        }
    }
}
