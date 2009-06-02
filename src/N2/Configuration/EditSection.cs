using System.Configuration;

namespace N2.Configuration
{
    /// <summary>
    /// Configuration related to the edit interface.
    /// </summary>
    public class EditSection : ConfigurationSection
    {
        [ConfigurationProperty("installer")]
        public InstallerElement Installer
        {
            get { return (InstallerElement)base["installer"]; }
            set { base["editTreeUrl"] = value; }
        }

        /// <summary>Users and roles considered editors.</summary>
        [ConfigurationProperty("editors")]
        public PermissionElement Editors
        {
            get { return (PermissionElement)base["editors"]; }
            set { base["editors"] = value; }
        }

        /// <summary>Users and roles considered administrators.</summary>
        [ConfigurationProperty("administrators")]
        public PermissionElement Administrators
        {
            get { return (PermissionElement)base["administrators"]; }
            set { base["administrators"] = value; }
		}

		/// <summary>Users and roles considered writers.</summary>
		[ConfigurationProperty("writers")]
		public PermissionElement Writers
		{
			get { return (PermissionElement)base["writers"]; }
			set { base["writers"] = value; }
		}
		
		[ConfigurationProperty("uploadFolders")]
        public FileSystemFolderCollection UploadFolders
        {
            get { return (FileSystemFolderCollection)base["uploadFolders"]; }
            set { base["uploadFolders"] = value; }
        }

		[ConfigurationProperty("editTreeUrl", DefaultValue = "~/edit/Navigation/Tree.aspx")]
        public string EditTreeUrl
        {
            get { return (string)base["editTreeUrl"]; }
            set { base["editTreeUrl"] = value; }
        }

        [ConfigurationProperty("editPreviewUrlFormat", DefaultValue = "{0}")]
        public string EditPreviewUrlFormat
        {
            get { return (string)base["editPreviewUrlFormat"]; }
            set { base["editPreviewUrlFormat"] = value; }
        }

        [ConfigurationProperty("editItemUrl", DefaultValue = "~/edit/edit.aspx")]
        public string EditItemUrl
        {
            get { return (string)base["editItemUrl"]; }
            set { base["editItemUrl"] = value; }
        }

        [ConfigurationProperty("editInterfaceUrl", DefaultValue = "~/edit/")]
        public string EditInterfaceUrl
        {
            get { return (string)base["editInterfaceUrl"]; }
            set { base["editInterfaceUrl"] = value; }
        }

        [ConfigurationProperty("newItemUrl", DefaultValue = "~/edit/new.aspx")]
        public string NewItemUrl
        {
            get { return (string)base["newItemUrl"]; }
            set { base["newItemUrl"] = value; }
        }

        [ConfigurationProperty("deleteItemUrl", DefaultValue = "~/edit/delete.aspx")]
        public string DeleteItemUrl
        {
            get { return (string)base["deleteItemUrl"]; }
            set { base["deleteItemUrl"] = value; }
        }

        [ConfigurationProperty("enableVersioning", DefaultValue = true)]
        public bool EnableVersioning
        {
            get { return (bool)base["enableVersioning"]; }
            set { base["enableVersioning"] = value; }
        }

        [ConfigurationProperty("tinyMCE")]
        public TinyMCEElement TinyMCE
        {
            get { return (TinyMCEElement)base["tinyMCE"]; }
            set { base["tinyMCE"] = value; }
        }

		[ConfigurationProperty("nameEditor")]
		public NameEditorElement NameEditor
		{
			get { return (NameEditorElement)base["nameEditor"]; }
			set { base["nameEditor"] = value; }
		}

		[ConfigurationProperty("settingsEditors")]
		public SettingsEditorCollection SettingsEditors
		{
			get { return (SettingsEditorCollection)base["settingsEditors"]; }
			set { base["settingsEditors"] = value; }
		}

		/// <summary>Information about default directories, usd to find default directory for a page.</summary>
		[ConfigurationProperty("defaultDirectory")]
		public DefaultDirectoryElement DefaultDirectory
		{
			get { return (DefaultDirectoryElement)base["defaultDirectory"]; }
			set { base["defaultDirectory"] = value; }
		}
    }
}
