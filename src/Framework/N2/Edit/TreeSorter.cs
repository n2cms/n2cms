using System;
using System.Collections.Generic;
using N2.Collections;
using N2.Engine;
using N2.Persistence;
using N2.Web;

namespace N2.Edit
{
	[Service(typeof(ITreeSorter))]
	public class TreeSorter : ITreeSorter
	{
		IPersister persister;
		IEditManager editManager;
		IWebContext webContext;

		public TreeSorter(IPersister persister, IEditManager editManager, IWebContext webContext)
		{
			this.persister = persister;
			this.editManager = editManager;
			this.webContext = webContext;
		}

		#region ITreeSorter Members

		public void MoveUp(ContentItem item)
		{
			if (item.Parent != null)
			{
				ItemFilter filter = editManager.GetEditorFilter(webContext.User);
				IList<ContentItem> siblings = item.Parent.Children;
				IList<ContentItem> filtered = new ItemList(siblings, filter);

				int index = filtered.IndexOf(item);
				if (index > 0)
				{
					MoveTo(item, NodePosition.Before, filtered[index - 1]);
				}
			}
		}

		public void MoveDown(ContentItem item)
		{
			if (item.Parent != null)
			{
				ItemFilter filter = editManager.GetEditorFilter(webContext.User);
				IList<ContentItem> siblings = item.Parent.Children;
				IList<ContentItem> filtered = new ItemList(siblings, filter);
				int index = filtered.IndexOf(item);
				if (index + 1 < filtered.Count)
				{
					MoveTo(item, NodePosition.After, filtered[index + 1]);
				}
			}
		}

		public void MoveTo(ContentItem item, int index)
		{
			IList<ContentItem> siblings = item.Parent.Children;
			Utility.MoveToIndex(siblings, item, index);
            using (var tx = persister.Repository.BeginTransaction())
            {
                foreach (ContentItem updatedItem in Utility.UpdateSortOrder(siblings))
                {
                    persister.Repository.SaveOrUpdate(updatedItem);
                }
                tx.Commit();
            }
		}

		public void MoveTo(ContentItem item, NodePosition position, ContentItem relativeTo)
		{
            if (relativeTo == null) throw new ArgumentNullException("item");
            if (relativeTo == null) throw new ArgumentNullException("relativeTo");
            if (relativeTo.Parent == null) throw new ArgumentException("The supplied item '" + relativeTo + "' has no parent to add to.", "relativeTo");
            
			if (item.Parent == null 
				|| item.Parent != relativeTo.Parent
				|| !item.Parent.Children.Contains(item))
				item.AddTo(relativeTo.Parent);

			IList<ContentItem> siblings = item.Parent.Children;
			
			int itemIndex = siblings.IndexOf(item);
			int relativeToIndex = siblings.IndexOf(relativeTo);
			
            if(itemIndex < 0)
            {
                if(position == NodePosition.Before)
                    siblings.Insert(relativeToIndex, item);
                else
                    siblings.Insert(relativeToIndex + 1, item);
            }
		    else if(itemIndex < relativeToIndex && position == NodePosition.Before)
				MoveTo(item, relativeToIndex - 1);
			else if (itemIndex > relativeToIndex && position == NodePosition.After)
				MoveTo(item, relativeToIndex + 1);
			else
				MoveTo(item, relativeToIndex);
		}
		#endregion
	}
}
