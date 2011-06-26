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

		#region IIdentifiable Members

		public object ElementKey
		{
			get { return Path; }
			set { Path = (string)value; }
		}

		#endregion
	}
}
