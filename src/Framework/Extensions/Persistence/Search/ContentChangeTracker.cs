using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;
using N2.Plugin;
using N2.Configuration;
using Lucene.Net.Store;
using N2.Web;
using System.Threading;
using System.Security;

namespace N2.Persistence.Search
{
	/// <summary>
	/// Observes changes in the content structure and invokes the <see cref="ILuceneIndexer"/> to perform indexing.
	/// </summary>
	[Service(Configuration = "lucene")]
	public class ContentChangeTracker : IAutoStart
	{
		IIndexer indexer;
		IPersister persister;
		IWorker worker;
		IErrorNotifier errors;
		bool async;
		bool handleErrors;

		public ContentChangeTracker(IIndexer indexer, IPersister persister, IWorker worker, ConnectionMonitor connection, IErrorNotifier errors, DatabaseSection config)
		{
			this.indexer = indexer;
			this.persister = persister;
			this.worker = worker;
			this.errors = errors;
			this.async = config.Search.AsyncIndexing;
			this.handleErrors = config.Search.HandleErrors;
			
			RetryInterval = TimeSpan.FromMinutes(2);

			if(config.Search.Enabled)
			{
				connection.Online += delegate
				{
					persister.ItemSaved += persister_ItemSaved;
					persister.ItemMoving += persister_ItemMoving;
					persister.ItemMoved += persister_ItemMoved;
					persister.ItemCopied += persister_ItemCopied;
					persister.ItemDeleted += persister_ItemDeleted;
				};
				connection.Offline += delegate
				{
					persister.ItemSaved -= persister_ItemSaved;
					persister.ItemMoving -= persister_ItemMoving;
					persister.ItemMoved -= persister_ItemMoved;
					persister.ItemCopied -= persister_ItemCopied;
					persister.ItemDeleted -= persister_ItemDeleted;
				};
			}
		}

		public TimeSpan RetryInterval { get; set; }

		public void ItemChanged(int itemID, bool affectsChildren)
		{
			if (itemID == 0)
				return;

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
			if (itemID == 0)
				return;

			DoWork(() =>
			{
				indexer.Delete(itemID);
			});
		}

		protected void DoWork(Action action)
		{
			if (handleErrors)
				action = WrapActionInErrorHandling(action);
			
			if (async)
				worker.DoWork(() =>
					{
						action();
						persister.Dispose();
					});
			else
				action();
		}

		Queue<Action> errorQueue = new Queue<Action>();
		DateTime lastAttempt = DateTime.Now;

		private Action WrapActionInErrorHandling(Action action)
		{
			var original = action;
			return () =>
			{
				try
				{
					original();
				}
				catch (LockObtainFailedException ex)
				{
					AppendError(original, ex);
				}
				catch (ThreadAbortException ex)
				{
					AppendError(original, ex);
				}
				catch (SecurityException ex)
				{
					AppendError(original, ex);
				}
				RetryFailedActions();
			};
		}

		private void AppendError(Action original, Exception ex)
		{
			errors.Notify(ex);
			lock (errorQueue)
			{
				errorQueue.Enqueue(original);
			}
		}

		private void RetryFailedActions()
		{
			if (errorQueue.Count == 0)
				return;

			if (Utility.CurrentTime().Subtract(lastAttempt) >= RetryInterval)
			{
				lock (errorQueue)
				{
					indexer.Unlock();
					lastAttempt = DateTime.Now;
					while (errorQueue.Count > 0)
						DoWork(errorQueue.Dequeue());
				}
			}
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
