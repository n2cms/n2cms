using System;
using System.Collections.Generic;
using N2.Edit.Versioning;
using System.Linq;

namespace N2.Persistence.Serialization
{
    public class ReadingJournal : IImportRecord
    {
		readonly IList<ContentItem> readItems = new List<ContentItem>();

        readonly IList<Attachment> attachments = new List<Attachment>();
        readonly IList<Attachment> failedAttachments = new List<Attachment>();
        readonly IList<Exception> errors = new List<Exception>();
	    readonly IList<Tuple<ContentItem, Exception>> failedContentItems = new List<Tuple<ContentItem, Exception>>();

	    public event EventHandler<ItemEventArgs> ItemAdded;

        public ReadingJournal()
        {
            UnresolvedLinks = new List<UnresolvedLink>();
        }

        public IList<ContentItem> ReadItems
        {
            get { return readItems; }
        }

        public ContentItem LastItem
        {
            get
            {
                if (readItems.Count == 0)
                    return null;
                return readItems[readItems.Count-1];
            }
        }

        public ContentItem RootItem
        {
            get
            {
                if(readItems.Count == 0)
                    return null;
                return readItems[0];
            }
        }

        public IList<Attachment> Attachments
        {
            get { return attachments; }
        }

        public IList<Attachment> FailedAttachments
        {
            get { return failedAttachments; }
        }


		public IList<Tuple<ContentItem, Exception>> FailedContentItems
		{
			get { return failedContentItems; }
		}

        public IList<Exception> Errors
        {
            get { return errors; }
        }

        public void Report(ContentItem item)
        {
            readItems.Add(item);
            if(ItemAdded != null)
                ItemAdded.Invoke(this, new ItemEventArgs(item));
        }

        public void Report(Attachment a)
        {
            attachments.Add(a);
        }

        public ContentItem Find(int itemiD)
        {
            foreach (ContentItem previousItem in readItems)
            {
                if (previousItem.ID == itemiD || previousItem.VersionOf.ID == itemiD)
                {
                    return previousItem;
                }
            }
            return null;
        }

        public void Error(Exception ex)
        {
            Errors.Add(ex);
        }

        public IList<UnresolvedLink> UnresolvedLinks { get; set; }
		
        public void RegisterParentRelation(int parentID, ContentItem item)
		{
			Register(parentID, (laterParent) => item.AddTo(laterParent), isChild: true, relationType: "parent", referencingItem: item);
		}

		public void Register(int referencedItemID, Action<ContentItem> action, bool isChild = false, string relationType = null, ContentItem referencingItem = null)
        {
			var resolver = new UnresolvedLink(referencedItemID, action) { IsChild = isChild, RelationType = relationType, ReferencingItem = referencingItem };
            UnresolvedLinks.Add(resolver);
            EventHandler<ItemEventArgs> handler = null;
            handler = delegate(object sender, ItemEventArgs e)
                {
                    if (e.AffectedItem.ID == referencedItemID || e.AffectedItem.VersionOf.ID == referencedItemID)
                    {
                        resolver.Setter(e.AffectedItem);
                        UnresolvedLinks.Remove(resolver);
                        ItemAdded -= handler;
                    }
                };

            ItemAdded += handler;
        }

        public void Register(string versionKey, Action<ContentItem> action, bool isChild = false)
        {
            var resolver = new UnresolvedLink(versionKey, action) { IsChild = isChild };
            UnresolvedLinks.Add(resolver);
            EventHandler<ItemEventArgs> handler = null;
            handler = delegate(object sender, ItemEventArgs e)
            {
                if (e.AffectedItem.GetVersionKey() == versionKey)
                {
                    resolver.Setter(e.AffectedItem);
                    UnresolvedLinks.Remove(resolver);
                    ItemAdded -= handler;
                }
            };

            ItemAdded += handler;
        }

        public ContentItem Find(string versionKey)
        {
            if (versionKey == null)
                return null;
            return ReadItems.FirstOrDefault(i => i.GetVersionKey() == versionKey);
        }


	}
}
