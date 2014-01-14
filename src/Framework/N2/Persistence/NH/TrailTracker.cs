using N2.Engine;
using N2.Plugin;

namespace N2.Persistence.NH
{
    /// <summary>
    /// Ensures that the ancestral trail on content items is up to date
    /// </summary>
    [Service(typeof(TrailTracker))]
    public class TrailTracker : IAutoStart
    {
        readonly IPersister persister;

        public TrailTracker(IPersister persister)
        {
            this.persister = persister;
        }

        void persister_ItemMoved(object sender, DestinationEventArgs e)
        {
            UpdateChildrenRecursiveAndSave(e.AffectedItem);
        }

        void persister_ItemCopied(object sender, DestinationEventArgs e)
        {
            UpdateChildrenRecursiveAndSave(e.AffectedItem);
        }

        void UpdateChildrenRecursiveAndSave(ContentItem parent)
        {
            int numberOfUpdatedItems = UpdateChildrenRecursive(parent);
            if (numberOfUpdatedItems == 0)
                return;

            using (var tx = persister.Repository.BeginTransaction())
            {
                if (parent is IActiveContent)
                    (parent as IActiveContent).Save();
                else
                    persister.Repository.SaveOrUpdate(parent);

                tx.Commit();
            }
        }

        int UpdateAncestralTrailOf(ContentItem item)
        {
            string trail = Utility.GetTrail(item.Parent);
            if (item.AncestralTrail != trail)
            {
                item.AncestralTrail = trail;
                return 1;
            }
            return 0;
        }

        private int UpdateChildrenRecursive(ContentItem parent)
        {
            int numberOfUpdatedItems = 0;
            foreach(ContentItem child in parent.Children)
            {
                numberOfUpdatedItems += UpdateAncestralTrailOf(child);
                numberOfUpdatedItems += UpdateChildrenRecursive(child);
            }
            return numberOfUpdatedItems;
        }

        #region IStartable Members

        public void Start()
        {
            persister.ItemMoved += persister_ItemMoved;
            persister.ItemCopied += persister_ItemCopied;
        }

        public void Stop()
        {
            persister.ItemMoved -= persister_ItemMoved;
            persister.ItemCopied -= persister_ItemCopied;
        }

        #endregion
    }
}
