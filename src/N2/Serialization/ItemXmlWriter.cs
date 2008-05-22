using System.Collections.Generic;
using System.Xml;
using N2.Definitions;
using N2.Web;

namespace N2.Serialization
{
	public class ItemXmlWriter : IXmlWriter
	{
		private readonly IEnumerable<IXmlWriter> writers;
		private readonly IDefinitionManager definitions;
		private readonly IUrlParser parser;

		public ItemXmlWriter(IDefinitionManager definitions, IUrlParser parser)
			: this(definitions, parser, DefaultWriters())
		{
		}

		public ItemXmlWriter(IDefinitionManager definitions, IUrlParser parser, IEnumerable<IXmlWriter> writers)
		{
			this.definitions = definitions;
			this.parser = parser;
			this.writers = writers;
		}

		public virtual void Write(ContentItem item, XmlTextWriter writer)
		{
			WriteSingleItem(item, writer);

			foreach(ContentItem child in item.Children)
			{
				Write(child, writer);
			}
		}

		private static IEnumerable<IXmlWriter> DefaultWriters()
		{
			return new IXmlWriter[]
				{
					new DetailXmlWriter(),
					new DetailCollectionXmlWriter(),
					new ChildXmlWriter(),
					new AuthorizationXmlWriter(),
					new AttachmentXmlWriter(new AttributeExplorer<IAttachmentHandler>())
				};
		}

		public virtual void WriteSingleItem(ContentItem item, XmlTextWriter writer)
		{
			using (ElementWriter itemElement = new ElementWriter("item", writer))
			{
				WriteDefaultAttributes(itemElement, item);

				foreach(IXmlWriter xmlWriter in writers)
				{
					xmlWriter.Write(item, writer);
				}
			}
		}

		protected virtual void WriteDefaultAttributes(ElementWriter itemElement, ContentItem item)
		{
			itemElement.WriteAttribute("id", item.ID);
			itemElement.WriteAttribute("name", item.Name);
			itemElement.WriteAttribute("parent", item.Parent != null ? item.Parent.ID.ToString() : string.Empty);
			itemElement.WriteAttribute("title", item.Title);
			itemElement.WriteAttribute("zoneName", item.ZoneName);
			itemElement.WriteAttribute("created", item.Created);
			itemElement.WriteAttribute("updated", item.Updated);
			itemElement.WriteAttribute("published", item.Published);
			itemElement.WriteAttribute("expires", item.Expires);
			itemElement.WriteAttribute("sortOrder", item.SortOrder);
			itemElement.WriteAttribute("url", parser.BuildUrl(item));
			itemElement.WriteAttribute("visible", item.Visible);
			itemElement.WriteAttribute("savedBy", item.SavedBy);
			itemElement.WriteAttribute("typeName", SerializationUtility.GetTypeAndAssemblyName(item.GetType()));
			itemElement.WriteAttribute("discriminator", definitions.GetDefinition(item.GetType()).Discriminator);
		}
	}
}