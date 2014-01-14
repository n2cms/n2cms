using System.Configuration;

namespace N2.Configuration
{
    public class ZipVppElement : NamedElement
    {
        /// <summary>The virtual location of the zip file.</summary>
        [ConfigurationProperty("filePath", DefaultValue = "")]
        public string FilePath
        {
            get { return (string)base["filePath"]; }
            set { base["filePath"] = value; }
        }

        /// <summary>The path below which resources may be retrieved from the zip file.</summary>
        [ConfigurationProperty("observedPath", DefaultValue = "")]
        public string ObservedPath
        {
            get { return (string)base["observedPath"]; }
            set { base["observedPath"] = value; }
        }
    }
}
