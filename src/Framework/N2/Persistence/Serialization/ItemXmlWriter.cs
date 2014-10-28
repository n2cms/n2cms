using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Xml;
using N2.Definitions;
using N2.Engine;
using N2.Web;
using N2.Edit.FileSystem;
using System.Linq;
using N2.Edit.Versioning;

namespace N2.Persistence.Serialization
{
    public interface IItemXmlWriter
    {
        void Write(ContentItem item, ExportOptions options, XmlTextWriter writer);
        void WriteSingleItem(ContentItem item, ExportOptions options, XmlTextWriter writer);
    }

    /// <summary>
    /// A content item xml serializer.
    /// </summary>
    [Service]
    [Service(typeof(IItemXmlWriter))]
    public class ItemXmlWriter : IItemXmlWriter
    {
        private readonly IDefinitionManager definitions;
        //private readonly IUrlParser parser;
        private readonly IFileSystem fs;

		public ItemXmlWriter(IDefinitionManager definitions, /*IUrlParser parser, */IFileSystem fs)
        {
            if (definitions == null)
                throw new ArgumentNullException("definitions");

            this.definitions = definitions;
            //this.parser = parser;
            this.fs = fs;
        }

        public virtual void Write(ContentItem item, ExportOptions options, XmlTextWriter writer)
        {
            WriteSingleItem(item, options, writer);

            foreach (ContentItem child in GetChildren(item, options))
            {
                Write(child, options, writer);
            }
        }

        internal static IEnumerable<ContentItem> GetChildren(ContentItem item, ExportOptions options)
        {
            if (!options.IsFlagSet(ExportOptions.ExcludePages) && !options.IsFlagSet(ExportOptions.ExcludeParts))
                return item.Children;
            else if (options.IsFlagSet(ExportOptions.ExcludePages))
                return item.Children.Where(c => !c.IsPage);
            else if (options.IsFlagSet(ExportOptions.ExcludeParts))
                return item.Children.Where(c => c.IsPage);
            return Enumerable.Empty<ContentItem>();
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
            yield return new ChildXmlWriter(options);
            yield return new AuthorizationXmlWriter();
            yield return new PersistablePropertyXmlWriter(definitions);
            if ((options & ExportOptions.ExcludeAttachments) == ExportOptions.Default)
                yield return new AttachmentXmlWriter(fs);
        }

        protected virtual void WriteDefaultAttributes(ElementWriter itemElement, ContentItem item)
        {
            if (itemElement == null)
                throw new ArgumentNullException("itemElement");
            if (item == null)
                throw new ArgumentNullException("item");

            itemElement.WriteAttribute("id", item.ID);
            itemElement.WriteAttribute("name", item.ID.ToString(CultureInfo.InvariantCulture) == item.Name ? "" : item.Name);
            if (item.Parent != null)
            {
                if (item.Parent.ID != 0)
                    itemElement.WriteAttribute("parent", item.Parent.ID.ToString(CultureInfo.InvariantCulture));
                else
                {
                    itemElement.WriteAttribute("parent", item.Parent.VersionOf.ID.ToString());
                    if (item.Parent.GetVersionKey() != null)
                        itemElement.WriteAttribute("parentVersionKey", item.Parent.GetVersionKey());
                }
            }

	        string typeAndAssemblyName = null;
	        try
	        {
				typeAndAssemblyName = SerializationUtility.GetTypeAndAssemblyName(item.GetContentType());
	        }
	        catch (Exception ex)
	        {
		        throw new Exception("Couldn't get type/assembly name.", ex);
	        }

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
            itemElement.WriteAttribute("url", item.Url);
            itemElement.WriteAttribute("visible", item.Visible);
            itemElement.WriteAttribute("savedBy", item.SavedBy);
            itemElement.WriteAttribute("typeName", typeAndAssemblyName);
            itemElement.WriteAttribute("discriminator", definitions.GetDefinition(item).Discriminator);
            itemElement.WriteAttribute("versionIndex", item.VersionIndex);
            itemElement.WriteAttribute("ancestralTrail", item.AncestralTrail);
            itemElement.WriteAttribute("alteredPermissions", (int)item.AlteredPermissions);
            itemElement.WriteAttribute("childState", (int)item.ChildState);
            if(item.VersionOf.HasValue)
            {
                Debug.Assert(item.VersionOf.ID != null, "item.VersionOf.ID != null");
                itemElement.WriteAttribute("versionOf", item.VersionOf.ID.Value);
            }
        }
    }
}
