using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;

namespace N2.Persistence.Serialization
{
	public class PersistablePropertyXmlReader : XmlReader, IXmlReader
	{
		#region IXmlReader Members

		public void Read(System.Xml.XPath.XPathNavigator navigator, ContentItem item, ReadingJournal journal)
		{
			foreach (XPathNavigator detailElement in EnumerateChildren(navigator))
			{
				ReadProperty(detailElement, item, journal);
			}
		}

		private void ReadProperty(XPathNavigator navigator, ContentItem item, ReadingJournal journal)
		{
			Dictionary<string, string> attributes = GetAttributes(navigator);

			string name = attributes["name"];

			if (!attributes.ContainsKey("typeName"))
			{
				item[name] = null;
				return;
			}

			Type type = Utility.TypeFromName(attributes["typeName"]);
			if(type == typeof(ContentItem))
				SetLinkedItem(navigator.Value, journal, (referencedItem) => item[name] = referencedItem);				
			else
				item[name] = Parse(navigator.Value, type);
		}

		#endregion
	}
}
