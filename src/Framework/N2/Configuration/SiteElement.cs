using System.Configuration;

namespace N2.Configuration
{
    public class SiteElement : ConfigurationElement
    {
        [ConfigurationProperty("id", IsRequired = true)]
        public int ID
        {
            get { return (int)base["id"]; }
            set { base["id"] = value; }
        }

        /// <summary>Optional configuration. When set this features allows multiple sites to coexist in the same database.</summary>
        [ConfigurationProperty("rootID")]
        public int? RootID
        {
            get { return (int?)base["rootID"]; }
            set { base["rootID"] = value; }
        }

        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)base["name"]; }
            set { base["name"] = value; }
        }

        /// <summary>Use wildcard matching for this hostname, e.g. both n2cms.com and www.n2cms.com.</summary>
        [ConfigurationProperty("wildcards", DefaultValue = false)]
        public bool Wildcards
        {
            get { return (bool)base["wildcards"]; }
            set { base["wildcards"] = value; }
        }

        /// <summary>Per site settings passed on to the site object.</summary>
        [ConfigurationProperty("settings")]
        public KeyValueConfigurationCollection Settings
        {
            get { return (KeyValueConfigurationCollection)base["settings"]; }
        }

        /// <summary>Upload folders to use in addition to those specified in the edit section. To use site specific folders only you can put a &lt;clear/&gt; in edit/uploadFolders.</summary>
        [ConfigurationProperty("uploadFolders")]
        public FileSystemFolderCollection UploadFolders
        {
            get { return (FileSystemFolderCollection)base["uploadFolders"]; }
            set { base["uploadFolders"] = value; }
        }
    }
}
