using System;
using System.Configuration;

namespace N2.Configuration
{
	/// <summary>
	/// Configuration related to the edit interface.
	/// </summary>
	public class EditSection : ConfigurationSectionBase
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

		[ConfigurationProperty("fileSystem")]
		public FileSystemElement FileSystem
		{
			get { return (FileSystemElement)base["fileSystem"]; }
			set { base["fileSystem"] = value; }
		}

		[ConfigurationProperty("editTreeUrl", DefaultValue = "{ManagementUrl}/Content/Navigation/Tree.aspx")]
		public string EditTreeUrl
		{
			get { return (string)base["editTreeUrl"]; }
			set { base["editTreeUrl"] = value; }
		}

		[ConfigurationProperty("editItemUrl", DefaultValue = "{ManagementUrl}/Content/Edit.aspx")]
		public string EditItemUrl
		{
			get { return (string)base["editItemUrl"]; }
			set { base["editItemUrl"] = value; }
		}

		[ConfigurationProperty("managementInterfaceUrl", DefaultValue = "~/N2/")]
		public string ManagementInterfaceUrl
		{
			get { return (string)base["managementInterfaceUrl"]; }
			set { base["managementInterfaceUrl"] = value; }
		}

		[ConfigurationProperty("editInterfaceUrl", DefaultValue = "{ManagementUrl}/Content/")]
		public string EditInterfaceUrl
		{
			get { return (string)base["editInterfaceUrl"]; }
			set { base["editInterfaceUrl"] = value; }
		}

		[ConfigurationProperty("newItemUrl", DefaultValue = "{ManagementUrl}/Content/New.aspx")]
		public string NewItemUrl
		{
			get { return (string)base["newItemUrl"]; }
			set { base["newItemUrl"] = value; }
		}

		[ConfigurationProperty("deleteItemUrl", DefaultValue = "{ManagementUrl}/Content/delete.aspx")]
		public string DeleteItemUrl
		{
			get { return (string)base["deleteItemUrl"]; }
			set { base["deleteItemUrl"] = value; }
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

		/// <summary>Information about default directories, used to find default directory for a page.</summary>
		[ConfigurationProperty("defaultDirectory")]
		public DefaultDirectoryElement DefaultDirectory
		{
			get { return (DefaultDirectoryElement)base["defaultDirectory"]; }
			set { base["defaultDirectory"] = value; }
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

		/// <summary>Information about versioning.</summary>
		[ConfigurationProperty("versions")]
		public VersionsElement Versions
		{
			get { return (VersionsElement)base["versions"]; }
			set { base["versions"] = value; }
		}

		/// <summary>Information about images.</summary>
		[ConfigurationProperty("images")]
		public ImagesElement Images
		{
			get { return (ImagesElement)base["images"]; }
			set { base["images"] = value; }
		}

		// deprecated

		[Obsolete("Use Versions.Enabled instead"), ConfigurationProperty("enableVersioning", DefaultValue = true)]
		public bool EnableVersioning
		{
			get { return Versions.Enabled; }
			set { Versions.Enabled = true; }
		}
	}
}