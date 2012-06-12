using System.Configuration;

namespace N2.Configuration
{
    public class FolderElement : ConfigurationElement, IIdentifiable
    {
        [ConfigurationProperty("path", IsKey = true)]
        public string Path
        {
            get { return (string)base["path"]; }
            set { base["path"] = value; }
        }

		[ConfigurationProperty("title")]
		public string Title
		{
			get { return (string)base["title"]; }
			set { base["title"] = value; }
		}

		/// <summary>Users and roles allowed to select files. File authorization for people browsing the site must be configured with ASP.NET location config.</summary>
		[ConfigurationProperty("readers")]
		public PermissionElement Readers
		{
			get { return (PermissionElement)base["readers"]; }
			set { base["readers"] = value; }
		}

		/// <summary>Users and roles allowed to upload files.</summary>
		[ConfigurationProperty("writers")]
		public PermissionElement Writers
		{
			get { return (PermissionElement)base["writers"]; }
			set { base["writers"] = value; }
		}

		#region IIdentifiable Members

		public object ElementKey
		{
			get { return Path; }
			set { Path = (string)value; }
		}

		#endregion
	}
}
