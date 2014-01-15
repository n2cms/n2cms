using System.Configuration;

namespace N2.Configuration
{
    /// <summary>
    /// Configuration related to default folder.
    /// </summary>
    public class DefaultDirectoryElement : ConfigurationElement
    {
        /// <summary>Controls how defualt folders are found.</summary>
        [ConfigurationProperty("mode", DefaultValue = DefaultDirectoryMode.RecursiveNames)]
        public DefaultDirectoryMode Mode
        {
            get { return (DefaultDirectoryMode)base["mode"]; }
            set { base["mode"] = value; }
        }

        /// <summary>The path to the default folder. When this property is null the first upload folder found is used instead.</summary>
        [ConfigurationProperty("rootPath")]
        public string RootPath
        {
            get { return (string)base["rootPath"]; }
            set { base["rootPath"] = value; }
        }
    }
}
