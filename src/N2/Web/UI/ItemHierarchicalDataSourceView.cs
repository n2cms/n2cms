using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace N2.Web.UI
{
	public class ItemHierarchicalDataSourceView : HierarchicalDataSourceView
	{
		public ItemHierarchicalDataSourceView(ContentItem parentItem)
		{
			this.parentItem = parentItem;
		}

		ContentItem parentItem;

		public override IHierarchicalEnumerable Select()
		{
			return parentItem != null ? parentItem.GetChildren() : null;
		}
	}
}
