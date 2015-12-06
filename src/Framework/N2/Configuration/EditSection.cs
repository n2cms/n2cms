using System;
using System.Configuration;

namespace N2.Configuration
{
    /// <summary>
    /// Configuration related to the edit interface.
    /// </summary>
    public class EditSection : ContentConfigurationSectionBase
    {
        [ConfigurationProperty("installer")]
        public InstallerElement Installer
        {
            get { return (InstallerElement)base["installer"]; }
            set { base["installer"] = value; }
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
        public RootFileSystemFolderCollection UploadFolders
        {
            get { return (RootFileSystemFolderCollection)base["uploadFolders"]; }
            set { base["uploadFolders"] = value; }
        }
        
        [ConfigurationProperty("fileSystem")]
        public FileSystemElement FileSystem
        {
            get { return (FileSystemElement)base["fileSystem"]; }
            set { base["fileSystem"] = value; }
        }

        [ConfigurationProperty("paths")]
        public PathsElement Paths
        {
            get { return (PathsElement)base["paths"]; }
            set { base["paths"] = value; }
        }

        /// <summary>
        /// The ASP.NET Theme to use for the Edit directory
        /// </summary>
        [ConfigurationProperty("theme")]
        public string EditTheme
        {
            get { return (string)base["theme"]; }
            set { base["theme"] = value; }
        }

		/// <summary>
		/// Configures if the edit toolbar is displayed on the bottom of the page. If false, the edit toolbar is placed at the top of the page.
		/// </summary>
		[ConfigurationProperty("toolbarOnBottom", DefaultValue = true)]
		public bool IsToolbarOnBottom
		{
			get { return (bool)base["toolbarOnBottom"]; }
			set { base["toolbarOnBottom"] = value; }
		}

        [ConfigurationProperty("ckeditor")]
        public CkEditorElement CkEditor
        {
            get { return (CkEditorElement)base["ckeditor"]; }
            set { base["ckeditor"] = value; }
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

        /// <summary>Configuration about default directories, used to find default directory for a page.</summary>
        [ConfigurationProperty("defaultDirectory")]
        public DefaultDirectoryElement DefaultDirectory
        {
            get { return (DefaultDirectoryElement)base["defaultDirectory"]; }
            set { base["defaultDirectory"] = value; }
        }

        /// <summary>Configuration about versioning.</summary>
        [ConfigurationProperty("versions")]
        public VersionsElement Versions
        {
            get { return (VersionsElement)base["versions"]; }
            set { base["versions"] = value; }
        }

        /// <summary>Configuration about images.</summary>
        [ConfigurationProperty("images")]
        public ImagesElement Images
        {
            get { return (ImagesElement)base["images"]; }
            set { base["images"] = value; }
        }

        /// <summary>Configuration about membership.</summary>
        [ConfigurationProperty("membership")]
        public MembershipElement Membership
        {
            get { return (MembershipElement)base["membership"]; }
            set { base["membership"] = value; }
        }

        /// <summary>Configuration about external items.</summary>
        [ConfigurationProperty("externals")]
        public ExternalsElement Externals
        {
            get { return (ExternalsElement)base["externals"]; }
            set { base["externals"] = value; }
        }

        /// <summary>Configuration about permanent redirect items.</summary>
        [ConfigurationProperty("linkTracker")]
        public LinkTrackerElement LinkTracker
        {
            get { return (LinkTrackerElement)base["linkTracker"]; }
            set { base["linkTracker"] = value; }
        }

        /// <summary>Configuration about editor collaboration features.</summary>
        [ConfigurationProperty("collaboration")]
        public CollaborationElement Collaboration
        {
            get { return (CollaborationElement)base["collaboration"]; }
            set { base["collaboration"] = value; }
        }

        /// <summary>Display legacy management interface.</summary>
        [ConfigurationProperty("legacy")]
        public bool Legacy
        {
            get { return (bool)base["legacy"]; }
            set { base["legacy"] = value; }
        }

        [ConfigurationProperty("docviewer")]
        public DocViewerElementCollection DocViewer
        {
            get { return (DocViewerElementCollection)base["docviewer"]; }
            set { base["docviewer"] = value; }
        }

		/// <summary>Configuration about editor collaboration features.</summary>
		[ConfigurationProperty("organize")]
		public OrganizeElement Organize
		{
			get { return (OrganizeElement)base["organize"]; }
			set { base["organize"] = value; }
		}
	}
}
