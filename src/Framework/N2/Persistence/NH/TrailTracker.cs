using N2.Edit;
using N2.Engine;
using N2.Plugin;
using System.Linq;

namespace N2.Persistence.NH
{
    /// <summary>
    /// Ensures that the ancestral trail on content items is up to date
    /// </summary>
    [Service(typeof(TrailTracker))]
    public class TrailTracker : IAutoStart
    {
        readonly IPersister persister;
		readonly ITreeSorter sorter;

        public TrailTracker(IPersister persister, ITreeSorter sorter)
        {
            this.persister = persister;
			this.sorter = sorter;
        }

        void persister_ItemMoved(object sender, DestinationEventArgs e)
        {
            UpdateChildrenRecursiveAndSave(e.AffectedItem);
        }

        void persister_ItemCopied(object sender, DestinationEventArgs e)
        {
            UpdateChildrenRecursiveAndSave(e.AffectedItem);
        }

		void sorter_ItemMoved(object sender, DestinationEventArgs e)
		{
			UpdateChildrenRecursiveAndSave(e.AffectedItem);
		}

        void UpdateChildrenRecursiveAndSave(ContentItem parent)
        {
			using (var tx = persister.Repository.BeginTransaction())
			{
				foreach (var updatedItem in Utility.UpdateAncestralTrailRecursive(parent, parent.Parent).ToList())
				{
					persister.Sources.Save(updatedItem);
				}

				tx.Commit();
			}
			//int numberOfUpdatedItems = UpdateChildrenRecursive(parent);
			//if (numberOfUpdatedItems == 0)
			//	return;

			//using (var tx = persister.Repository.BeginTransaction())
			//{
			//	if (parent is IActiveContent)
			//		(parent as IActiveContent).Save();
			//	else
			//		persister.Repository.SaveOrUpdate(parent);

			//	tx.Commit();
			//}
        }

		//int UpdateAncestralTrailOf(ContentItem item)
		//{
		//	string trail = Utility.GetTrail(item.Parent);
		//	if (item.AncestralTrail != trail)
		//	{
		//		item.AncestralTrail = trail;
		//		return 1;
		//	}
		//	return 0;
		//}

		//private int UpdateChildrenRecursive(ContentItem parent)
		//{
		//	int numberOfUpdatedItems = 0;
		//	foreach(ContentItem child in parent.Children)
		//	{
		//		numberOfUpdatedItems += UpdateAncestralTrailOf(child);
		//		numberOfUpdatedItems += UpdateChildrenRecursive(child);
		//	}
		//	return numberOfUpdatedItems;
		//}

        #region IStartable Members

        public void Start()
        {
            persister.ItemMoved += persister_ItemMoved;
            persister.ItemCopied += persister_ItemCopied;
			sorter.ItemMoved += sorter_ItemMoved;
        }

        public void Stop()
        {
            persister.ItemMoved -= persister_ItemMoved;
            persister.ItemCopied -= persister_ItemCopied;
			sorter.ItemMoved -= sorter_ItemMoved;
		}

        #endregion
    }
}
