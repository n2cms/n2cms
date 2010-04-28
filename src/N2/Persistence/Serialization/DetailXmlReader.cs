using System;
using System.Collections.Generic;
using System.Xml.XPath;
using N2.Details;

namespace N2.Persistence.Serialization
{
	/// <summary>
	/// Reads a content detail from the input navigator.
	/// </summary>
	public class DetailXmlReader : XmlReader, IXmlReader
	{
		string applicationPath = N2.Web.Url.ApplicationPath ?? "/";

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
				object value = Parse(navigator.Value, type);
				if (value is string)
					value = PrepareStringDetail(item, name, value as string);

				item.SetDetail(name, value, type);
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

		private object PrepareStringDetail(ContentItem item, string name, string value)
		{
			if (value.StartsWith("~"))
			{
				var pi = item.GetType().GetProperty(name);
				if (pi != null)
				{
					var transformers = pi.GetCustomAttributes(typeof(IRelativityTransformer), false);
					foreach (IRelativityTransformer transformer in transformers)
					{
						if(transformer.RelativeWhen == RelativityMode.ExportRelativeImportAbsolute)
							value = transformer.ToAbsolute(applicationPath, value);
					}
				}
			}
			return value;
		}
	}
}
