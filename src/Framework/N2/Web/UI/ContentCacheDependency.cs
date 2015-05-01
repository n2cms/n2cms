using System;
using System.Web.Caching;
using N2.Persistence;

namespace N2.Web.UI
{
	public class PageContentCacheDependency : ContentCacheDependency
	{
		private int invalidateOnItemWithID;

		public PageContentCacheDependency(IPersister persister, int invalidateOnItemWithID)
			: base(persister)
		{
			this.invalidateOnItemWithID = invalidateOnItemWithID;
		}

		protected override void NotifyDependencyChanged(object sender, ItemEventArgs e)
		{
			if (e.AffectedItem.ID == invalidateOnItemWithID)
				base.NotifyDependencyChanged(sender, e);
		}
	}

	public class SectionContentCacheDependency : ContentCacheDependency
	{
		private string invalidatePagesBelowTrail;

		public SectionContentCacheDependency(IPersister persister, string invalidatePagesBelowTrail)
			: base(persister)
		{
			this.invalidatePagesBelowTrail = invalidatePagesBelowTrail;
		}

		protected override void NotifyDependencyChanged(object sender, ItemEventArgs e)
		{
			if (e.AffectedItem.GetTrail().StartsWith(invalidatePagesBelowTrail))
				base.NotifyDependencyChanged(sender, e);
		}
	}

    public class ContentCacheDependency : CacheDependency
    {
        IPersister persister;

        public ContentCacheDependency(IPersister persister)
        {
            this.persister = persister;
            persister.ItemMoved += new EventHandler<DestinationEventArgs>(persister_ItemMoved);
            persister.ItemSaved += new EventHandler<ItemEventArgs>(persister_ItemSaved);
            persister.ItemDeleted += new EventHandler<ItemEventArgs>(persister_ItemDeleted);
        }

        protected override void DependencyDispose()
        {
            persister.ItemMoved -= new EventHandler<DestinationEventArgs>(persister_ItemMoved);
            persister.ItemSaved -= new EventHandler<ItemEventArgs>(persister_ItemSaved);
            persister.ItemDeleted -= new EventHandler<ItemEventArgs>(persister_ItemDeleted);
        }

		protected virtual void NotifyDependencyChanged(object sender, ItemEventArgs e)
		{
			base.NotifyDependencyChanged(sender, e);
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
