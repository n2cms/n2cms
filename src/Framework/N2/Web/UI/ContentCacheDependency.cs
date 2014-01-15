using System;
using System.Web.Caching;
using N2.Persistence;

namespace N2.Web.UI
{
    public class ContentCacheDependency : CacheDependency
    {
        IPersister persister;

        public ContentCacheDependency(IPersister persister)
        {
            this.persister = persister;
            persister.ItemMoved += new EventHandler<DestinationEventArgs>(persister_ItemMoved);
            persister.ItemSaved += new EventHandler<ItemEventArgs>(persister_ItemSaved);
            persister.ItemDeleted += new EventHandler<ItemEventArgs>(persister_ItemDeleted);
            persister.ItemCopied += new EventHandler<DestinationEventArgs>(persister_ItemCopied);
        }

        protected override void DependencyDispose()
        {
            persister.ItemMoved -= new EventHandler<DestinationEventArgs>(persister_ItemMoved);
            persister.ItemSaved -= new EventHandler<ItemEventArgs>(persister_ItemSaved);
            persister.ItemDeleted -= new EventHandler<ItemEventArgs>(persister_ItemDeleted);
            persister.ItemCopied -= new EventHandler<DestinationEventArgs>(persister_ItemCopied);
        }

        void persister_ItemCopied(object sender, DestinationEventArgs e)
        {
            NotifyDependencyChanged(sender, e);
        }

        void persister_ItemDeleted(object sender, ItemEventArgs e)
        {
            NotifyDependencyChanged(sender, e);
        }

        void persister_ItemSaved(object sender, ItemEventArgs e)
        {
            NotifyDependencyChanged(sender, e);
        }

        void persister_ItemMoved(object sender, DestinationEventArgs e)
        {
            NotifyDependencyChanged(sender, e);
        }
    }
}
