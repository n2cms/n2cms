using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Caching;
using N2.Persistence;
using System.Diagnostics;

namespace N2.Web.Caching
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
