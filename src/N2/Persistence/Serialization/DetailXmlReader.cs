using System;
using System.Collections.Generic;
using System.Xml.XPath;

namespace N2.Persistence.Serialization
{
	/// <summary>
	/// Reads a content detail from the input navigator.
	/// </summary>
	public class DetailXmlReader : XmlReader, IXmlReader
	{
		public void Read(XPathNavigator navigator, ContentItem item, ReadingJournal journal)
		{
			foreach (XPathNavigator detailElement in EnumerateChildren(navigator))
			{
				ReadDetail(detailElement, item, journal);
			}
		}

		protected virtual void ReadDetail(XPathNavigator navigator, ContentItem item, ReadingJournal journal)
		{
			Dictionary<string, string> attributes = GetAttributes(navigator);
			Type type = Utility.TypeFromName(attributes["typeName"]);

			string name = attributes["name"];

			if (type != typeof(ContentItem))
			{
				item.SetDetail(name, Parse(navigator.Value, type), type);
			}
			else
			{
				int referencedItemID = int.Parse(navigator.Value);
				ContentItem referencedItem = journal.Find(referencedItemID);
				if (referencedItem != null)
				{
					item[name] = referencedItem;
				}
				else
				{
					EventHandler<ItemEventArgs> handler = null;
					handler = delegate(object sender, ItemEventArgs e)
					{
						if (e.AffectedItem.ID == referencedItemID)
						{
							item[name] = e.AffectedItem;
							journal.ItemAdded -= handler;
						}
					};
					
					journal.ItemAdded += handler;
				}
			}
		}
	}
}
