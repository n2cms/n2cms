using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using N2.Definitions;
using N2.Details;
using N2.Engine;
using N2.Persistence.Serialization;
using N2.Web;
using N2.Edit.FileSystem;
using System.Linq;
using N2.Edit.Versioning;

namespace N2.Persistence.Xml
{
	//[Service]
	//[Service(typeof(IItemXmlWriter))]
    public class ItemHtmlWriter : IItemXmlWriter
    {

        private static DetailXmlWriter dww = new DetailXmlWriter();
        private static DetailCollectionXmlWriter dw = new DetailCollectionXmlWriter();

        private readonly IDefinitionManager _definitions;
        private readonly IFileSystem _fs;


        public ItemHtmlWriter(IDefinitionManager definitions, IFileSystem fs)
        {
            _definitions = definitions;
            _fs = fs;
        }

        /// <summary>
        /// Writes a page out to the given XmlTextWriter
        /// </summary>
        /// <param name="item"></param>
        /// <param name="options"></param>
        /// <param name="writer"></param>
        public virtual void Write(ContentItem item, ExportOptions options, XmlTextWriter writer)
        {
            Debug.Assert(item.IsPage, "item.IsPage");
            using (var html = new ElementWriter("html", writer))
            {
                writer.WriteStartElement("head");
                writer.WriteEndElement();
                writer.WriteStartElement("body");

                WriteSingleItem(item, options, writer);

                WriteChildPartsWithZones(item, options, writer); 
                writer.WriteEndElement(); // </body>
            }
        }

        public virtual void WriteChildPartsWithZones(ContentItem item, ExportOptions options, XmlTextWriter writer)
        {
            var itemsInZones = item.Children.GroupBy(x => x.ZoneName);
            foreach (var zone in itemsInZones)
                using (var zoneElement = new ElementWriter("section", writer))
                {
                    zoneElement.WriteAttribute("class", "zone");
                    zoneElement.WriteAttribute("id", zone.Key);

                    foreach (var childItem in zone)
                        using (var partElement = new ElementWriter("part", writer))
                        {
                            partElement.WriteAttribute("id", childItem.ID);
                            WriteSingleItem(childItem, options, writer);
                            WriteChildPartsWithZones(childItem, options, writer); // recursively until there are no more zones
                        }
                }
        }

        public virtual void WriteSingleItem(ContentItem item, ExportOptions options, XmlTextWriter writer)
        {
            var authorizedRolesList = String.Join("|",
                                                  from x in item.AuthorizedRoles
                                                  let y = x.ToString()
                                                  select y);

            using (var properties = new ElementWriter("ul", writer))
            {
                properties.WriteAttribute("class", "n2-itemproperties");
                properties.WriteAttribute("id", "n2-item" + item.ID);
                WriteSingleDetail(writer, "id", item.ID);
                WriteSingleDetail(writer, "name", item.ID.ToString() == item.Name ? "" : item.Name);

                if (item.Parent != null)
                {
                    if (item.Parent.ID != 0)
                        WriteSingleDetail(writer, "parent", item.Parent.ID.ToString());
                    else
                    {
                        WriteSingleDetail(writer, "parent", item.Parent.VersionOf.ID.ToString());
                        if (item.Parent.GetVersionKey() != null)
                            WriteSingleDetail(writer, "parentVersionKey", item.Parent.GetVersionKey());
                    }
                }
                WriteSingleDetail(writer, "title", item.Title);
                WriteSingleDetail(writer, "zoneName", item.ZoneName);
                WriteSingleDetail(writer, "templateKey", item.TemplateKey);
                WriteSingleDetail(writer, "translationKey", item.TranslationKey ?? 0);
                WriteSingleDetail(writer, "state", item.State.ToString());
                WriteSingleDetail(writer, "created", item.Created);
                WriteSingleDetail(writer, "updated", item.Updated);
                WriteSingleDetail(writer, "published", item.Published);
                WriteSingleDetail(writer, "expires", item.Expires);
                WriteSingleDetail(writer, "sortOrder", item.SortOrder);
                WriteSingleDetail(writer, "url", item.Url);
                WriteSingleDetail(writer, "visible", item.Visible);
                WriteSingleDetail(writer, "savedBy", item.SavedBy);
                WriteSingleDetail(writer, "typeName", SerializationUtility.GetTypeAndAssemblyName(item.GetContentType()));
                WriteSingleDetail(writer, "discriminator", _definitions.GetDefinition(item).Discriminator);
                WriteSingleDetail(writer, "versionIndex", item.VersionIndex);
                WriteSingleDetail(writer, "ancestralTrail", item.AncestralTrail);
                WriteSingleDetail(writer, "alteredPermissions", item.AlteredPermissions.ToString());
                WriteSingleDetail(writer, "childState", item.ChildState.ToString());
                if (item.VersionOf.HasValue)
                {
                    Debug.Assert(item.VersionOf.ID != null, "item.VersionOf.ID != null");
                    WriteSingleDetail(writer, "versionOf", item.VersionOf.ID.Value);
                }

                WriteSingleDetail(writer, "authorizedRoles", authorizedRolesList);
                //WriteSingleDetail(writer, "Extension", item.Extension);
                //WriteSingleDetail(writer, "IconUrl", item.IconUrl);
                //WriteSingleDetail(writer, "IsExpired", item.IsExpired());
                //WriteSingleDetail(writer, "IsPage", item.IsPage);
                //WriteSingleDetail(writer, "Path", item.Path);


                //foreach (ContentDetail d in item.Details)
                //  using (var html = new ElementWriter(C_PROPERTYTAG, writer))
                //  {
                //      html.WriteAttribute("data-type", d.ValueTypeKey);
                //      html.WriteAttribute("data-name", d.Name);
                //      html.WriteCData(d.Value.ToString());
                //  }
            }

            dw.Write(item, writer);
            dww.Write(item, writer);
        }

        #region WriteSingleDetail

        private const string CPropertytag = "property";


        private static void WriteSingleDetail<T>(XmlTextWriter writer, string key, T detail)
        {
            var type = SerializationUtility.GetTypeAndAssemblyName(typeof(T));
            using (var td = new ElementWriter("li", writer))
            {
                td.WriteAttribute("data-key", key);
                td.WriteAttribute("data-type", type);
                try { td.WriteCData(detail.ToString()); }
                catch { td.Write("NULL"); }
            }
        }

        #endregion


    }
}
