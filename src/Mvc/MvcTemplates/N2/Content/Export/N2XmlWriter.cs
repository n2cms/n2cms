using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using N2.Collections;
using N2.Persistence.Serialization;

namespace N2.Xml
{
    public class N2XmlWriter
    {
        #region Constructor
        public N2XmlWriter(N2.Engine.IEngine engine)
        {
            this.engine = engine;
        } 
        #endregion

        #region Fields
        N2.Engine.IEngine engine;
        private bool useUniversalTime = false;
        private bool includeChildren = true;
        private IList<N2.Collections.ItemFilter> filters = new List<N2.Collections.ItemFilter>();
        private Formatting xmlFormatting = Formatting.Indented; 
        #endregion

        #region Properties
        public Formatting XmlFormatting
        {
            get { return xmlFormatting; }
            set { xmlFormatting = value; }
        }

        protected IList<N2.Collections.ItemFilter> Filters
        {
            get { return filters; }
            set { filters = value; }
        }

        public bool IncludeChildren
        {
            get { return includeChildren; }
            set { includeChildren = value; }
        }

        public bool UseUniversalTime
        {
            get { return useUniversalTime; }
            set { useUniversalTime = value; }
        } 
        #endregion
        
        public void Write(N2.ContentItem rootItem, System.IO.TextWriter output)
        {
            XmlTextWriter xtw = new XmlTextWriter(output);
            xtw.Formatting = this.XmlFormatting;
            xtw.WriteStartDocument();

            xtw.WriteStartElement("n2");
            xtw.WriteAttributeString("version", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            xtw.WriteAttributeString("exportVersion", "1");
            xtw.WriteAttributeString("exportDate", GetDateTimeString(N2.Utility.CurrentTime()));

            OnWritingItem(rootItem, xtw);

            xtw.WriteEndElement();
            xtw.WriteEndDocument();
        }

        #region Write Helper Methods
        protected virtual void OnWritingItem(N2.ContentItem item, XmlTextWriter xtw)
        {
            xtw.WriteStartElement("item");
            OnWritingDefaultAttributes(item, xtw);

            OnWritingAuthorizedRoles(item, xtw);
            OnWritingDetails(item, xtw);
            OnWritingDetailCollections(item, xtw);
            OnWritingChildren(item, xtw);

            xtw.WriteEndElement();
        }

        protected virtual void OnWritingDefaultAttributes(N2.ContentItem item, XmlTextWriter xtw)
        {
            xtw.WriteAttributeString("id", item.ID.ToString());
            xtw.WriteAttributeString("name", item.Name);
            xtw.WriteAttributeString("title", item.Title);
            xtw.WriteAttributeString("zoneName", item.ZoneName);
            xtw.WriteAttributeString("created", GetDateTimeString(item.Created));
            xtw.WriteAttributeString("updated", GetDateTimeString(item.Updated));
            xtw.WriteAttributeString("published", (item.Published.HasValue) ? GetDateTimeString(item.Published.Value) : string.Empty);
            xtw.WriteAttributeString("expires", (item.Expires.HasValue) ? GetDateTimeString(item.Expires.Value) : string.Empty);
            xtw.WriteAttributeString("sortOrder", item.SortOrder.ToString());
            xtw.WriteAttributeString("url", engine.UrlParser.BuildUrl(item));
            xtw.WriteAttributeString("visible", item.Visible.ToString());
            xtw.WriteAttributeString("savedBy", item.SavedBy);
            xtw.WriteAttributeString("typeName", SerializationUtility.GetTypeAndAssemblyName(item.GetContentType()));
            xtw.WriteAttributeString("discriminator", engine.Definitions.GetDefinition(item).Discriminator);
        }

        protected virtual void OnWritingAuthorizedRoles(ContentItem item, XmlTextWriter xtw)
        {
            xtw.WriteStartElement("authorizedRoles");
            foreach (N2.Security.AuthorizedRole role in item.AuthorizedRoles)
                xtw.WriteElementString("role", role.Role);
            xtw.WriteEndElement();
        }

        protected virtual void OnWritingDetails(ContentItem item, XmlTextWriter xtw)
        {
            xtw.WriteStartElement("details");
            foreach (N2.Details.ContentDetail detail in item.Details)
            {
                OnWritingDetail(xtw, detail);
            }
            xtw.WriteEndElement();
        }

        protected virtual void OnWritingDetail(XmlTextWriter xtw, N2.Details.ContentDetail detail)
        {
            xtw.WriteStartElement("detail");
            xtw.WriteAttributeString("name", detail.Name);
            xtw.WriteAttributeString("typeName", SerializationUtility.GetTypeAndAssemblyName(detail.ValueType));
            if (detail.ValueType == typeof(object))
            {
                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream stream = new MemoryStream();
                bf.Serialize(stream, detail.Value);
                xtw.WriteString(Convert.ToBase64String(stream.ToArray()));
            }
            else if (detail.ValueType == typeof(ContentItem))
            {
                xtw.WriteString(detail.LinkedItem.ID.ToString());
            }
            else if (detail.Value == typeof(string))
                xtw.WriteCData(detail.StringValue);
            else
                xtw.WriteString(detail.Value.ToString());
            xtw.WriteEndElement();
        }

        protected virtual void OnWritingDetailCollections(ContentItem item, XmlTextWriter xtw)
        {
            xtw.WriteStartElement("detailCollections");
            foreach (N2.Details.DetailCollection collection in item.DetailCollections.Values)
            {
                xtw.WriteStartElement("details");
                xtw.WriteAttributeString("name", collection.Name);
                foreach (N2.Details.ContentDetail detail in collection.Details)
                {
                    OnWritingDetail(xtw, detail);
                }
                xtw.WriteEndElement();
            }
            xtw.WriteEndElement();
        }

        protected virtual void OnWritingChildren(N2.ContentItem item, XmlTextWriter xtw)
        {
            xtw.WriteStartElement("children");
            if (IncludeChildren)
                foreach (ContentItem child in GetChildren(item))
                    OnWritingItem(child, xtw);
            xtw.WriteEndElement();
        }

        protected virtual IEnumerable<ContentItem> GetChildren(ContentItem item)
        {
            return new N2.Collections.ItemList(item.Children, new AllFilter(Filters));
        }

        protected string GetDateTimeString(DateTime date)
        {
            if (useUniversalTime)
                date = date.ToUniversalTime();
            return date.ToString();
        } 
        #endregion

        public StringBuilder GetXml(N2.ContentItem rootItem)
        {
            StringBuilder sb = new StringBuilder();
            using (System.IO.StringWriter sw = new System.IO.StringWriter(sb))
            {
                Write(rootItem, sw);
            }
            return sb;
        }
    }
}
