using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;
using N2.Plugin;
using N2.Configuration;
using N2.Web;
using System.Threading;
using System.Security;

namespace N2.Persistence.Search
{
    /// <summary>
    /// Observes changes in the content structure and invokes the <see cref="IAsyncIndexer"/> to perform indexing.
    /// </summary>
    [Service]
    public class ContentChangeTracker : IAutoStart
    {
        IAsyncIndexer indexer;

        public bool IsMonitoring { get; private set; }

        public ContentChangeTracker(IAsyncIndexer indexer, IPersister persister, ConnectionMonitor connection, DatabaseSection config)
        {
            this.indexer = indexer;

            if (!config.Search.Enabled)
                return;
            if (!string.IsNullOrEmpty(config.Search.IndexOnMachineNamed) && config.Search.IndexOnMachineNamed != Environment.MachineName)
                return;

            connection.Online += delegate
            {
                IsMonitoring = true;
                persister.ItemSaved += persister_ItemSaved;
                persister.ItemMoving += persister_ItemMoving;
                persister.ItemMoved += persister_ItemMoved;
                persister.ItemCopied += persister_ItemCopied;
                persister.ItemDeleted += persister_ItemDeleted;
            };
            connection.Offline += delegate
            {
                IsMonitoring = false;
                persister.ItemSaved -= persister_ItemSaved;
                persister.ItemMoving -= persister_ItemMoving;
                persister.ItemMoved -= persister_ItemMoved;
                persister.ItemCopied -= persister_ItemCopied;
                persister.ItemDeleted -= persister_ItemDeleted;
            };
        }


        public void ItemChanged(int itemID, bool affectsChildren)
        {
            if (itemID == 0)
                return;

            indexer.Reindex(itemID, affectsChildren);
        }

        public void ItemDeleted(int itemID)
        {
            if (itemID == 0)
                return;

            indexer.Delete(itemID);
        }



        #region IAutoStart Members

        public void Start()
        {
        }

        public void Stop()
        {
        }

        void persister_ItemDeleted(object sender, ItemEventArgs e)
        {
            ItemDeleted(e.AffectedItem.ID);
        }

        void persister_ItemCopied(object sender, DestinationEventArgs e)
        {
            ItemChanged(e.AffectedItem.ID, true);
        }

        void persister_ItemMoving(object sender, CancellableDestinationEventArgs e)
        {
            var originalAction = e.FinalAction;
            e.FinalAction = (from, to) =>
            {
                var result = originalAction(from, to);
                ItemDeleted(from.ID);
                return result;
            };
        }

        void persister_ItemMoved(object sender, DestinationEventArgs e)
        {
            ItemChanged(e.AffectedItem.ID, true);
        }

        void persister_ItemSaved(object sender, ItemEventArgs e)
        {
            ItemChanged(e.AffectedItem.ID, false);
        }
        #endregion
    }
}
