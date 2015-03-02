using System;
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
        public int ID { get; set; }
        public int VersionIndex { get; set; }
        public string Title { get; set; }
        public ContentRelation Master { get; set; }
        public ContentState State { get; set; }
        public DateTime? Published { get; set; }
        public DateTime? FuturePublish { get; set; }
        public DateTime? Expired { get; set; }
        public DateTime Saved { get; set; }
        public string SavedBy { get; set; }
		public int ItemCount { get; set; }
		public string VersionDataXml { get; set; }

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
    }
}
