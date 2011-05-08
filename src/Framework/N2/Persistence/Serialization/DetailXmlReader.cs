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

			if (type == typeof(ContentItem))
			{
				SetLinkedItem(navigator.Value, journal, (referencedItem) => item[name] = referencedItem);
			}
			else if(type == typeof(IMultipleValue))
			{
				var multiDetail = ReadMultipleValue(navigator, item, journal, name);
				multiDetail.AddTo(item);
			}
			else
			{
				object value = Parse(navigator.Value, type);
				if (value is string)
					value = PrepareStringDetail(item, name, value as string);

				item[name] = value;
				//item.SetDetail(name, value, type);
			}
		}

		internal ContentDetail ReadMultipleValue(XPathNavigator navigator, ContentItem item, ReadingJournal journal, string name)
		{
			var multiDetail = ContentDetail.Multi(name);
			foreach (XPathNavigator valueElement in EnumerateChildren(navigator))
			{
				switch (valueElement.GetAttribute("key", ""))
				{
					case ContentDetail.TypeKeys.BoolType:
						multiDetail.BoolValue = (bool)Parse(valueElement.Value, typeof(bool));
						break;
					case ContentDetail.TypeKeys.DateTimeType:
						multiDetail.DateTimeValue = (DateTime)Parse(valueElement.Value, typeof(DateTime));
						break;
					case ContentDetail.TypeKeys.DoubleType:
						multiDetail.DoubleValue = (double)Parse(valueElement.Value, typeof(double));
						break;
					case ContentDetail.TypeKeys.IntType:
						multiDetail.IntValue = (int)Parse(valueElement.Value, typeof(int));
						break;
					case ContentDetail.TypeKeys.LinkType:
						SetLinkedItem(valueElement.Value, journal, (referencedItem) => multiDetail.LinkedItem = referencedItem);
						break;
					case ContentDetail.TypeKeys.MultiType:
						journal.Error(new InvalidOperationException("Nested multi types not supported"));
						break;
					case ContentDetail.TypeKeys.ObjectType:
						multiDetail.ObjectValue = Parse(valueElement.Value, typeof(object));
						break;
					case ContentDetail.TypeKeys.StringType:
						multiDetail.StringValue = (string)PrepareStringDetail(item, name, valueElement.Value);
						break;
				}
			}
			return multiDetail;
		}

		private static void SetLinkedItem(string value, ReadingJournal journal, Action<ContentItem> setter)
		{
			int referencedItemID = int.Parse(value);
			ContentItem referencedItem = journal.Find(referencedItemID);
			if (referencedItem != null)
			{
				setter(referencedItem);
			}
			else
			{
				EventHandler<ItemEventArgs> handler = null;
				handler = delegate(object sender, ItemEventArgs e)
				{
					if (e.AffectedItem.ID == referencedItemID)
					{
						setter(e.AffectedItem);
						journal.ItemAdded -= handler;
					}
				};

				journal.ItemAdded += handler;
			}
		}

		private string PrepareStringDetail(ContentItem item, string name, string value)
		{
			if (value.StartsWith("~"))
			{
				var pi = item.GetContentType().GetProperty(name);
				if (pi != null)
				{
					var transformers = pi.GetCustomAttributes(typeof(IRelativityTransformer), false);
					foreach (IRelativityTransformer transformer in transformers)
					{
						if (transformer.RelativeWhen == RelativityMode.Always || transformer.RelativeWhen == RelativityMode.ImportingOrExporting)
							value = transformer.Rebase(value, "~/", applicationPath);
					}
				}
			}
			return value;
		}
	}
}
