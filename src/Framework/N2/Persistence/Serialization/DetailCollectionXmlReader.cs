using System;
using System.Collections.Generic;
using System.Xml.XPath;
using N2.Details;

namespace N2.Persistence.Serialization
{
    public class DetailCollectionXmlReader : XmlReader, IXmlReader
    {
        DetailXmlReader detailReader = new DetailXmlReader();

        public void Read(XPathNavigator navigator, ContentItem item, ReadingJournal journal)
        {
            foreach (XPathNavigator detailCollectionElement in EnumerateChildren(navigator))
            {
                ReadDetailCollection(detailCollectionElement, item, journal);
            }
        }

        protected void ReadDetailCollection(XPathNavigator navigator, ContentItem item, ReadingJournal journal)
        {
            Dictionary<string, string> attributes = GetAttributes(navigator);
            string name = attributes["name"];

            foreach (XPathNavigator detailElement in EnumerateChildren(navigator))
            {
                ReadDetail(detailElement, item.GetDetailCollection(name, true), journal);
            }
        }

        protected virtual void ReadDetail(XPathNavigator navigator, DetailCollection collection, ReadingJournal journal)
        {
            Dictionary<string, string> attributes = GetAttributes(navigator);
            Type type = Utility.TypeFromName(attributes["typeName"]);

			string meta = attributes.ContainsKey("meta") ? attributes["meta"] : null;
            if (type == typeof(ContentItem))
            {
                int referencedItemID = int.Parse(navigator.Value);
                ContentItem referencedItem = journal.Find(referencedItemID);
                if (referencedItem != null)
                {
                    collection.Add(ContentDetail.New(
                        collection.EnclosingItem,
                        attributes["name"],
                        referencedItem, 
						meta));
                }
                else
                {
                    journal.Register(referencedItemID, (item) => 
                        {
                            collection.Add(ContentDetail.New(
                                collection.EnclosingItem,
                                attributes["name"],
                                item,
								meta));
                        }, relationType: "collectionlink");
                }
            }
            else if (type == typeof(Enum))
            {
                if (meta != null)
                {
                    collection.Add(ContentDetail.New(
                        collection.EnclosingItem,
                        attributes["name"],
                        Parse(navigator.Value, Type.GetType(meta))));
                }
            }
            else if (type == typeof(IMultipleValue))
            {
				var detail = detailReader.ReadMultipleValue(navigator, collection.EnclosingItem, journal, collection.Name);
				detail.Meta = meta;
				detail.AddTo(collection);
            }
            else
            {
                object value = Parse(navigator.Value, type);
                if (value is string)
                    value = detailReader.PrepareStringDetail(collection.EnclosingItem, collection.Name, value as string, attributes.ContainsKey("encoded") && Convert.ToBoolean(attributes["encoded"]));

                collection.Add(ContentDetail.New(collection.EnclosingItem, attributes["name"], value, meta));
            }
        }

    }
}
