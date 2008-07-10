using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Web.Configuration;

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
        public EditAccess Editors
        {
            get { return (EditAccess)base["editors"]; }
            set { base["editors"] = value; }
        }

        /// <summary>Users and roles considered administrators.</summary>
        [ConfigurationProperty("administrators")]
        public EditAccess Administrators
        {
            get { return (EditAccess)base["administrators"]; }
            set { base["administrators"] = value; }
        }

        [ConfigurationProperty("editTreeUrl", DefaultValue = "Navigation/Tree.aspx")]
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

        [ConfigurationProperty("editorCssUrl", DefaultValue = "~/Edit/Css/Editor.css")]
        public string EditorCssUrl
        {
            get { return (string)base["editorCssUrl"]; }
            set { base["editorCssUrl"] = value; }
        }

        [ConfigurationProperty("uploadFolderUrl", DefaultValue = "~/Upload")]
        public string UploadFolderUrl
        {
            get { return (string)base["uploadFolderUrl"]; }
            set { base["uploadFolderUrl"] = value; }
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
    }
}
