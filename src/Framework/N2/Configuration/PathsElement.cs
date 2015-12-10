using System.Configuration;

namespace N2.Configuration
{
	public class PathsElement : ConfigurationElement
	{
		[ConfigurationProperty("selectedQueryKey", DefaultValue = "selected")]
		public string SelectedQueryKey
		{
			get { return (string)base["selectedQueryKey"]; }
			set { base["selectedQueryKey"] = value; }
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

		[ConfigurationProperty("editInterfaceUrl", DefaultValue = "{ManagementUrl}/")]
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

        [ConfigurationProperty("mediaBrowserUrl", DefaultValue = "{ManagementUrl}/Content/Navigation/MediaBrowser.aspx")]
        public string MediaBrowserUrl
        {
            get { return (string)base["mediaBrowserUrl"]; }
            set { base["mediaBrowserUrl"] = value; }
        }

        [ConfigurationProperty("ensureLocalhostPreviewUrls", DefaultValue = false)]
		public bool EnsureLocalhostPreviewUrls
		{
			get { return (bool)base["ensureLocalhostPreviewUrls"]; }
			set { base["ensureLocalhostPreviewUrls"] = value; }
		}

		[ConfigurationProperty("setSecurityUrl", DefaultValue = "{ManagementUrl}/Content/Security/Default.aspx")]
		public string SetSecurityUrl
		{
			get { return (string)base["setSecurityUrl"]; }
			set { base["setSecurityUrl"] = value; }
		}

	}
}
