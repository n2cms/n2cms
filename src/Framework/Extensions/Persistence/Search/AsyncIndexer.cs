using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;
using N2.Configuration;
using N2.Web;
using Lucene.Net.Store;
using System.Threading;
using System.Security;

namespace N2.Persistence.Search
{
    [Service(typeof(IAsyncIndexer))]
    public class AsyncIndexer : IAsyncIndexer
    {
        IIndexer indexer;
        IPersister persister;
        IErrorNotifier errors;
        IWorker worker;
		Logger<AsyncIndexer> logger;
		public int maxErrorsPerWorkItem = 3;

		protected class Work
		{
			public string Name { get; set; }
			public Action Action { get; set; }
			public int ErrorCount { get; set; }
		}

        Queue<Work> workQueue = new Queue<Work>();
        DateTime lastAttempt = DateTime.Now;
        bool async;
        bool handleErrors;
        bool isWorking;
        string currentWork = "";

        public TimeSpan RetryInterval { get; set; }

        public AsyncIndexer(IIndexer indexer, IPersister persister, IWorker worker, IErrorNotifier errors, DatabaseSection config)
        {
            RetryInterval = TimeSpan.FromMinutes(2);
            this.async = config.Search.AsyncIndexing;
            this.handleErrors = config.Search.HandleErrors;

            this.indexer = indexer;
            this.persister = persister;
            this.worker = worker;
            this.errors = errors;
        }

        protected void Execute(Work work)
        {
            if (handleErrors)
                WrapActionInErrorHandling(work);

            if (!async)
			{
                work.Action();
				return;
			}

			lock (workQueue)
			{
				logger.Debug("Enqueuing " + work.Name + ", to existing: " + workQueue.Count + " on thread " + Thread.CurrentThread.ManagedThreadId);
				workQueue.Enqueue(work);
			}

			ExecuteEnqueuedActionsAsync();
        }

		private void ExecuteEnqueuedActionsAsync()
		{
			lock (workQueue)
			{
				// only 1 worker
				if (isWorking)
					return;
				isWorking = true;
				
				worker.DoWork(() =>
				{
					try
					{
						while (true)
						{
							// we want a new session on the thread after each action avoid creating big NH sessions
							using (persister)
							{
								// work while there is remaining work
								Work work = null;
								lock (workQueue)
								{
									if (workQueue.Count <= 0)
										// all done, we can end
										return;

									// next thing to do
									work = workQueue.Dequeue();
									logger.Debug("Dequeuing " + work.Name + ", remaining: " + workQueue.Count + " on thread " + Thread.CurrentThread.ManagedThreadId);
								}

								logger.Debug("Executing " + work.Name);
								work.Action();
							}
						}
					}
					finally
					{
						lock (workQueue)
						{
							// allow new worker to be spawned
							isWorking = false;
							currentWork = "";
						}
					}
				});
			}
		}

        private void WrapActionInErrorHandling(Work action)
        {
            var original = action.Action;
			action.Action = () =>
			{
				try
				{
					original();
				}
				catch (LockObtainFailedException ex)
				{
					AppendError(action, ex);
				}
				catch (ThreadAbortException ex)
				{
					AppendError(action, ex);
				}
				catch (SecurityException ex)
				{
					AppendError(action, ex);
				}
				catch (Exception ex)
				{
					AppendError(action, ex);
				}

				if (async)
					RetryFailedActions();
				else if (action.ErrorCount < maxErrorsPerWorkItem)
					original();
			};
        }


        private void AppendError(Work work, Exception ex)
        {
			logger.Error("Error while processing " + work.Name);
			errors.Notify(ex);
            lock (workQueue)
            {
				work.ErrorCount++;
				if (work.ErrorCount < maxErrorsPerWorkItem)
					workQueue.Enqueue(work);
				else
					logger.WarnFormat("Maximum numer of errors per work item ({0}) reached, dropping work '{1}'", work.ErrorCount, work.Name);
            }
        }

        private void RetryFailedActions()
        {
			lock (workQueue)
			{
				if (workQueue.Count == 0)
					return;

				if (Utility.CurrentTime().Subtract(lastAttempt) >= RetryInterval)
				{
                    indexer.Unlock();
                    lastAttempt = DateTime.Now;
					ExecuteEnqueuedActionsAsync();
				}
			}
		}

		public virtual void ReindexDescendants(int rootID, bool clearBeforeReindex)
        {
			Execute(new Work
			{
				Name = "Reindex descendants of #" + rootID,
				Action = () =>
				{
					if (clearBeforeReindex)
						indexer.Delete(rootID);

					Reindex(rootID, true);
				}
			});
        }

        public virtual void Reindex(int itemID, bool affectsChildren)
        {
			Execute(new Work
				{
					Name = "Reindex #" + itemID + " (affectsChildren = " + affectsChildren + ")",
					Action = () =>
					{
						// get the item to update using a session on this thread
						var item = persister.Get(itemID);
						if (item == null)
							return;

						// update the index
						currentWork = "Indexing " + item.Title + " #" + itemID;
						indexer.Update(item);

						var childrenIdsToReindex = new List<int>();
						if (affectsChildren)
						{
							// get a list of child ids to update, and wait until session is closed to schedule reindexing
							foreach (var child in item.Children)
							{
								childrenIdsToReindex.Add(child.ID);
							}
						}

						foreach (var childId in childrenIdsToReindex)
						{
							Reindex(childId, affectsChildren);
						}
					}
				});
        }

        public virtual void Delete(int itemID)
        {
			Execute(new Work
				{
					Name = "Delete #" + itemID,
					Action = () =>
					{
						currentWork = "Deleting #" + itemID;
						indexer.Delete(itemID);
					}
				});
        }

        public IndexStatus GetCurrentStatus()
        {
            return new IndexStatus
            {
                CurrentWork = currentWork,
                WorkerCount = isWorking ? 1 : 0,
				QueueSize = workQueue.Count
            };
        }
	}
}
