using System.Collections.Generic;
using System.Xml;
using N2.Definitions;
using N2.Engine;
using N2.Web;
using N2.Edit.FileSystem;

namespace N2.Persistence.Serialization
{
    /// <summary>
    /// A content item xml serializer.
    /// </summary>
	[Service]
	public class ItemXmlWriter
	{
		private readonly IDefinitionManager definitions;
		private readonly IUrlParser parser;
		private readonly IFileSystem fs;

		public ItemXmlWriter(IDefinitionManager definitions, IUrlParser parser, IFileSystem fs)
		{
			this.definitions = definitions;
			this.parser = parser;
			this.fs = fs;
		}

        public virtual void Write(ContentItem item, ExportOptions options, XmlTextWriter writer)
		{
			WriteSingleItem(item, options, writer);

			foreach(ContentItem child in item.Children)
			{
				if (child.ID != 0)
					Write(child, options, writer);
			}
		}

		public virtual void WriteSingleItem(ContentItem item, ExportOptions options, XmlTextWriter writer)
		{
			using (ElementWriter itemElement = new ElementWriter("item", writer))
			{
				WriteDefaultAttributes(itemElement, item);

				foreach(IXmlWriter xmlWriter in GetWriters(options))
				{
					xmlWriter.Write(item, writer);
				}
			}
		}

        private IEnumerable<IXmlWriter> GetWriters(ExportOptions options)
        {
            if((options & ExportOptions.OnlyDefinedDetails) == ExportOptions.OnlyDefinedDetails)
                yield return new DefinedDetailXmlWriter(definitions);
            else
                yield return new DetailXmlWriter();
			yield return new DetailCollectionXmlWriter();
			yield return new ChildXmlWriter();
			yield return new AuthorizationXmlWriter();
			yield return new PersistablePropertyXmlWriter(definitions);
            if ((options & ExportOptions.ExcludeAttachments) == ExportOptions.Default)
			    yield return new AttachmentXmlWriter(fs);
        }

		protected virtual void WriteDefaultAttributes(ElementWriter itemElement, ContentItem item)
		{
			itemElement.WriteAttribute("id", item.ID);
			itemElement.WriteAttribute("name", item.ID.ToString() == item.Name ? "" : item.Name);
			itemElement.WriteAttribute("parent", item.Parent != null ? item.Parent.ID.ToString() : string.Empty);
			itemElement.WriteAttribute("title", item.Title);
			itemElement.WriteAttribute("zoneName", item.ZoneName);
			itemElement.WriteAttribute("templateKey", item.TemplateKey);
			if (item.TranslationKey.HasValue)
				itemElement.WriteAttribute("translationKey", item.TranslationKey.Value);
			itemElement.WriteAttribute("state", (int)item.State);
			itemElement.WriteAttribute("created", item.Created);
			itemElement.WriteAttribute("updated", item.Updated);
			itemElement.WriteAttribute("published", item.Published);
			itemElement.WriteAttribute("expires", item.Expires);
			itemElement.WriteAttribute("sortOrder", item.SortOrder);
			itemElement.WriteAttribute("url", parser.BuildUrl(item));
			itemElement.WriteAttribute("visible", item.Visible);
			itemElement.WriteAttribute("savedBy", item.SavedBy);
			itemElement.WriteAttribute("typeName", SerializationUtility.GetTypeAndAssemblyName(item.GetContentType()));
			itemElement.WriteAttribute("discriminator", definitions.GetDefinition(item).Discriminator);
		}
	}
}