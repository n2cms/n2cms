using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Collections;

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
			referrers.AddRange(Find.Items.Where.Detail().Eq(Item).Select());
			foreach (ContentItem child in current.GetChildren())
			{
				AddReferencesRecursive(child, referrers);
			}
		}
	}
}