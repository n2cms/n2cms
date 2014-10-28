using System;
using N2.Collections;
using N2.Persistence;

namespace N2.Edit
{
    public partial class ReferencingItems : System.Web.UI.UserControl
    {
        private N2.ContentItem item;
        public N2.ContentItem Item
        {
            get { return item; }
            set { item = value; }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            if (Item == null || Item.ID == 0)
                return;

            ItemList referrers = new ItemList();
            AddReferencesRecursive(Item, referrers);
            N2.Collections.DuplicateFilter.FilterDuplicates(referrers);
            this.rptItems.DataSource = referrers;
        }

        protected void AddReferencesRecursive(ContentItem current, ItemList referrers)
        {
            referrers.AddRange(Content.Search.Repository.Find(Parameter.Equal(null, Item).Detail()));
			foreach (ContentItem child in current.Children.WhereAccessible())
            {
                AddReferencesRecursive(child, referrers);
            }
        }
    }
}
