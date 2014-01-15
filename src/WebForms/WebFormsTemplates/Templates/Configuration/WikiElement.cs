using System.Configuration;

namespace N2.Templates.Configuration
{
    public class WikiElement : ConfigurationElement
    {
        /// <summary>Use free text editor or regular textarea.</summary>
        [ConfigurationProperty("freeTextMode", DefaultValue = false)]
        public bool FreeTextMode
        {
            get { return (bool)base["freeTextMode"]; }
            set { base["freeTextMode"] = value; }
        }
    }
}
