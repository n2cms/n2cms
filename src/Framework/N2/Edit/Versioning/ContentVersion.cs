using System;
using System.Xml.Serialization;
using N2.Persistence;
using N2.Persistence.Serialization;
using N2.Engine;
using System.IO;
using System.Linq;
using N2.Web;

namespace N2.Edit.Versioning
{
    public sealed class ContentVersion
    {
        private Func<string, ContentItem> _deserializer;
        private Func<ContentItem, string> _serializer;
        private string _versionDataXml;
        private ContentItem _version;

        public ContentVersion()
        {
        }

        public ContentVersion(Importer importer, Exporter exporter, IUrlParser parser)
        {
            Deserializer = xml => Deserialize(importer, parser, xml);
            Serializer = item => Serialize(exporter, item);
        }

        // ReSharper disable RedundantNameQualifier
        [XmlIgnore]
        public Func<string, ContentItem> Deserializer
        {
            get
            {
                return _deserializer 
                    ?? (_deserializer = xml => Deserialize(N2.Context.Current.Resolve<Importer>(), N2.Context.Current.UrlParser, xml));
            }
            set { _deserializer = value; }
        }

        [XmlIgnore]
        public Func<ContentItem, string> Serializer
        {
            get 
            { 
                return _serializer 
                    ?? (_serializer = item => Serialize(N2.Context.Current.Resolve<Exporter>(), item));
            }
            set { _serializer = value; }
        }
        // ReSharper restore RedundantNameQualifier

        public int ID { get; set; }
        public int VersionIndex { get; set; }
        public string Title { get; set; }
        public ContentRelation Master { get; set; }
        public ContentState State { get; set; }
        public DateTime? Published { get; set; }
        public DateTime? FuturePublish { get; set; }
        public DateTime? Expired { get; set; }
        //public virtual string PublishedBy { get; set; }
        public DateTime Saved { get; set; }
        public string SavedBy { get; set; }

        public string VersionDataXml
        {
            get { return _versionDataXml; }
            set { _versionDataXml = value; _version = null; }
        }

        public ContentItem Version
        {
            get
            {
                if (string.IsNullOrEmpty(VersionDataXml))
                    return null;

                if (_version != null)
                    return _version;
                
                _version = Deserializer(VersionDataXml);
                if (FuturePublish.HasValue)
                    _version["FuturePublishDate"] = FuturePublish;
                _version.Updated = Saved;
                return _version;
            }
            set
            {
                _version = value;

                if (value == null)
                {
                    Published = null;
                    FuturePublish = null;
                    Expired = null;
                    VersionDataXml = null;
                    VersionIndex = 0;
                    Title = null;
                    State = ContentState.None;
                    //PublishedBy = null;
                    ItemCount = 0;
                    return;
                }

                VersionIndex = value.VersionIndex;
                Published = value.Published;
                FuturePublish = value["FuturePublishDate"] as DateTime?;
                if (FuturePublish.HasValue)
                    value["FuturePublishDate"] = null;
                Expired = value.Expires;
                SavedBy = value.SavedBy;
                Title = value.Title;
                State = value.State;
                //PublishedBy = value.IsPublished() ? value.SavedBy : null;
                VersionDataXml = Serializer(value);
            }
        }

        internal static ContentItem Deserialize(Importer importer, IUrlParser parser, string xml)
        {
            if (importer == null)
                throw new ArgumentException("Importer cannot be null.", "importer");
            
            if (parser == null)
                throw new ArgumentException("Parser cannot be null.", "parser");

            if (String.IsNullOrEmpty(xml))
                return null; // nothing to deserialize

            var journal = importer.Read(new StringReader(xml));
            foreach (var link in journal.UnresolvedLinks.Where(ul => ul.IsChild == false))
            {
                var item = importer.Persister.Get(link.ReferencedItemID);
                if (item != null)
                    link.Setter(item);
            }

            if (journal.ReadItems == null)
                throw new Exception("Journal couldn't read items due to journal.ReadItems == null. " + xml);

            try
            {
                foreach (var item in journal.ReadItems)
                    (item as IInjectable<IUrlParser>).Set(parser);
            }
            catch (NullReferenceException nilX)
            {
                throw new Exception("Ran into a null reference while attempting to read items from the journal: " + xml, nilX);
            }

            if (journal.RootItem == null)
                return null;

            if (journal.RootItem.VersionOf.HasValue && journal.RootItem.VersionOf.Value != null)
                journal.RootItem.Parent = journal.RootItem.VersionOf.Parent;

            ReorderBySortOrderRecursive(journal.RootItem);

            return journal.RootItem;
        }

        private static void ReorderBySortOrderRecursive(ContentItem item)
        {
            if (item.Children.Count > 0)
            {
                item.Children = new Collections.ItemList(item.Children.OrderBy(i => i.SortOrder));
                foreach (var child in item.Children)
                    ReorderBySortOrderRecursive(child);
            }
        }

        internal static string Serialize(Exporter exporter, ContentItem item)
        {
            using (var sw = new StringWriter())
            {
                exporter.Export(item, ExportOptions.ExcludePages | ExportOptions.ExcludeAttachments, sw);
                return sw.ToString();
            }
        }

        public int ItemCount { get; set; }
    }
}
