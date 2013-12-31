using System;
using System.Xml.XPath;
using N2.Engine;

namespace N2.Persistence.Serialization.Xml
{
    [Service]
    [Service(typeof(IItemXmlReader))]
    public class ItemHtmlReader : IItemXmlReader
    {
        public bool IgnoreMissingTypes { get; set; }
        
        public IImportRecord Read(XPathNavigator navigator)
        {
            throw new NotImplementedException();
        }

    }
}
