using System;
using System.Collections.Generic;
using System.Xml.XPath;
using N2.Definitions;

namespace N2.Serialization
{
	public class ItemXmlReader : XmlReader
	{
		private readonly IDefinitionManager definitions;
		private readonly IDictionary<string, IXmlReader> readers;

		public ItemXmlReader(IDefinitionManager definitions)
			: this(definitions, DefaultReaders())
		{
		}

		public ItemXmlReader(IDefinitionManager definitions, IDictionary<string, IXmlReader> readers)
		{
			this.definitions = definitions;
			this.readers = readers;
		}

		private static IDictionary<string, IXmlReader> DefaultReaders()
		{
			IDictionary<string, IXmlReader> readers = new Dictionary<string, IXmlReader>();
			readers["details"] = new DetailXmlReader();
			readers["detailCollections"] = new DetailCollectionXmlReader();
			readers["authorizations"] = new AuthorizationXmlReader();
			readers["attachments"] = new AttachmentXmlReader(new AttributeExplorer<IAttachmentHandler>());
			return readers;
		}

		public virtual IImportRecord Read(XPathNavigator navigator)
		{
			if (navigator == null) throw new ArgumentNullException("navigator");

			ReadingJournal journal = new ReadingJournal();
			foreach (XPathNavigator itemElement in EnumerateChildren(navigator))
			{
				ContentItem item = ReadSingleItem(itemElement, journal);
				journal.Report(item);
			}
			return journal;
		}

		public virtual ContentItem ReadSingleItem(XPathNavigator navigator, ReadingJournal journal)
		{
			if (navigator.LocalName != "item") throw new DeserializationException("Expected element 'item' but was '" + navigator.LocalName + "'");

			Dictionary<string, string> attributes = GetAttributes(navigator);
			ContentItem item = CreateInstance(attributes);
			ReadDefaultAttributes(attributes, item, journal);

			foreach(XPathNavigator current in EnumerateChildren(navigator))
			{
				if(readers.ContainsKey(current.LocalName))
					readers[current.LocalName].Read(current, item, journal);
			}

			return item;
		}

		protected virtual void ReadDefaultAttributes(Dictionary<string, string> attributes, ContentItem item, ReadingJournal journal)
		{
            item.Created = ToNullableDateTime(attributes["created"]).Value;
			item.Expires = ToNullableDateTime(attributes["expires"]);
			item.ID = Convert.ToInt32(attributes["id"]);
			item.Name = attributes["name"];
			item.Published = ToNullableDateTime(attributes["published"]);
			item.SavedBy = attributes["savedBy"];
			item.SortOrder = Convert.ToInt32(attributes["sortOrder"]);
			item.Title = attributes["title"];
            item.Updated = ToNullableDateTime(attributes["updated"]).Value;
			item.Visible = Convert.ToBoolean(attributes["visible"]);
			if (!string.IsNullOrEmpty(attributes["zoneName"]))
				item.ZoneName = attributes["zoneName"];
			HandleParentRelation(item, attributes["parent"], journal);
		}

		protected virtual void HandleParentRelation(ContentItem item, string parent, ReadingJournal journal)
		{
			if (!string.IsNullOrEmpty(parent))
			{
				int parentID = int.Parse(parent);
				ContentItem parentItem = journal.Find(parentID);
				item.AddTo(parentItem);
			}
		}

		private ContentItem CreateInstance(Dictionary<string, string> attributes)
		{
			ItemDefinition definition = FindDefinition(attributes);
			return definitions.CreateInstance(definition.ItemType, null);
		}

		protected virtual ItemDefinition FindDefinition(Dictionary<string, string> attributes)
		{
			string discriminator = attributes["discriminator"];
			foreach (ItemDefinition d in definitions.GetDefinitions())
				if (d.Discriminator == discriminator)
					return d;
			throw new DefinitionNotFoundException("No definition found for discriminator: " + discriminator);
		}
	}
}
