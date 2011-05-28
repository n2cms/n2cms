using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;
using N2.Plugin;
using N2.Configuration;

namespace N2.Persistence.Search
{
	/// <summary>
	/// Observes changes in the content structure and invokes the <see cref="ILuceneIndexer"/> to perform indexing.
	/// </summary>
	[Service]
	public class ContentChangeTracker : IAutoStart
	{
		IIndexer indexer;
		IPersister persister;
		IWorker worker;
		bool async;
		bool enabled;

		public ContentChangeTracker(IIndexer indexer, IPersister persister, IWorker worker, DatabaseSection config)
		{
			this.indexer = indexer;
			this.persister = persister;
			this.worker = worker;
			this.async = config.Search.AsyncIndexing;
			this.enabled = config.Search.Enabled;
		}

		public void ItemChanged(int itemID, bool affectsChildren)
		{
			DoWork(() =>
			{
				var item = persister.Get(itemID);
				indexer.Update(item);

				if (affectsChildren)
				{
					foreach (var descendant in Find.EnumerateChildren(item))
					{
						indexer.Update(descendant);
					}
				}
			});
		}

		public void ItemDeleted(int itemID)
		{
			DoWork(() =>
			{
				indexer.Delete(itemID);
			});
		}

		protected void DoWork(Action action)
		{
			if (async)
				worker.DoWork(() =>
					{
						action();
						persister.Dispose();
					});
			else
				action();
		}

		#region IAutoStart Members

		public void Start()
		{
			if (!enabled)
				return;

			persister.ItemSaved += persister_ItemSaved;
			persister.ItemMoved += persister_ItemMoved;
			persister.ItemCopied += persister_ItemCopied;
			persister.ItemDeleted += persister_ItemDeleted;
		}

		public void Stop()
		{
			if (!enabled)
				return;

			persister.ItemSaved -= persister_ItemSaved;
			persister.ItemMoving -= persister_ItemMoving;
			persister.ItemMoved -= persister_ItemMoved;
			persister.ItemCopied -= persister_ItemCopied;
			persister.ItemDeleted -= persister_ItemDeleted;
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
