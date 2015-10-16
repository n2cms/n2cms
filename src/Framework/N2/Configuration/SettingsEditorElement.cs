using System.Configuration;

namespace N2.Configuration
{
    public class SettingsEditorElement : ConfigurationElement, IIdentifiable
    {
        public SettingsEditorElement()
        {
        }

        public SettingsEditorElement(string name, string path)
        {
            Name = name;
            Path = path;
        }

        [ConfigurationProperty("name", IsKey = true)]
        public string Name
        {
            get { return (string)base["name"]; }
            set { base["name"] = value; }
        }

        [ConfigurationProperty("path")]
        public string Path
        {
            get { return (string)base["path"]; }
            set { base["path"] = value; }
        }

        #region IIdentifiable Members

        public object ElementKey
        {
            get { return Name; }
            set { Name = (string)value; }
        }

        #endregion
    }
}
