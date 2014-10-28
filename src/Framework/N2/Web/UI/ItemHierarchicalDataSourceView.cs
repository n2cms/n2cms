using System;
using System.Web.UI;

namespace N2.Web.UI
{
	[Obsolete]
    public class ItemHierarchicalDataSourceView : HierarchicalDataSourceView
    {
        public ItemHierarchicalDataSourceView(ContentItem parentItem)
        {
            this.parentItem = parentItem;
        }

        ContentItem parentItem;

        public override IHierarchicalEnumerable Select()
        {
            return parentItem != null ? new HierarchyEnumerator(parentItem) : null;
        }

        class HierarchyEnumerator : IHierarchicalEnumerable
        {
            private ContentItem parentItem;

            public HierarchyEnumerator(ContentItem parentItem)
            {
                this.parentItem = parentItem;
            }

            #region IHierarchicalEnumerable Members

            public IHierarchyData GetHierarchyData(object enumeratedItem)
            {
                return new ItemHierarchyData((ContentItem)enumeratedItem);
            }

            #endregion

            #region Nested type: ItemHierarchyData

            private class ItemHierarchyData : IHierarchyData
            {
                private ContentItem item;

                public ItemHierarchyData(ContentItem item)
                {
                    this.item = item;
                }

                #region IHierarchyData Members

                IHierarchicalEnumerable IHierarchyData.GetChildren()
                {
                    return new HierarchyEnumerator(item);
                }

                IHierarchyData IHierarchyData.GetParent()
                {
                    return (item.Parent != null)
                            ? new ItemHierarchyData(item.Parent)
                            : null;
                }

                bool IHierarchyData.HasChildren
                {
                    get { return item.GetChildPagesUnfiltered().WhereNavigatable().Count > 0; }
                }

                object IHierarchyData.Item
                {
                    get { return item; }
                }

                string IHierarchyData.Path
                {
                    get { return item.Url; }
                }

                string IHierarchyData.Type
                {
                    get { return item.GetContentType().Name; }
                }

                #endregion
            }

            #endregion

            public System.Collections.IEnumerator GetEnumerator()
            {
                return this.parentItem.GetChildPagesUnfiltered().WhereNavigatable().GetEnumerator();
            }
        }

    }
}
