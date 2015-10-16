using System.Configuration;
using System.Collections.Specialized;
using System.ComponentModel;

namespace N2.Configuration
{
    public class ZipVppCollection : LazyRemovableCollection<ZipVppElement>
    {
        [ConfigurationProperty("staticFileExtensions", DefaultValue = ".js,.css,.gif,.png,.jpg,.htm,.html,.swf")]
        [TypeConverter(typeof(CommaDelimitedStringCollectionConverter))]
        public StringCollection StaticFileExtensions
        {
            get { return (CommaDelimitedStringCollection)base["staticFileExtensions"]; }
            set { base["staticFileExtensions"] = value; }
        }
    }
}
