using System;
using System.Configuration;

namespace N2.Configuration
{
    public class FileSystemElement : ConfigurationElement
    {
        [ConfigurationProperty("textFileExtensions", DefaultValue = ".cs,.vb,.js,.html,.htm,.xml,.aspx,.ascx,.ashx,.php,.txt")]
        public string TextFileExtensions
        {
            get { return (string)base["textFileExtensions"]; }
            set { base["textFileExtensions"] = value; }
        }

        [ConfigurationProperty("imageFileExtensions", DefaultValue = ".jpg,.gif,.jpeg,.png")]
        public string ImageFileExtensions
        {
            get { return (string)base["imageFileExtensions"]; }
            set { base["imageFileExtensions"] = value; }
        }

        public bool IsTextFile(string virtualPath)
        {
            return Array.FindIndex(
                TextFileExtensions.Split(','),
                (extension) => virtualPath.EndsWith(extension, StringComparison.InvariantCultureIgnoreCase)) >= 0;
        }
    }
}
