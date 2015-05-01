using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;
using N2.Engine;
using N2.Persistence.Serialization;

namespace N2.Persistence.Xml
{
	//[Service]
	//[Service(typeof(IItemXmlReader))]
    public class ItemHtmlReader : IItemXmlReader
    {
        public bool IgnoreMissingTypes { get; set; }
        
        public IImportRecord Read(XPathNavigator navigator)
        {
            throw new NotImplementedException();
        }

    }
}
