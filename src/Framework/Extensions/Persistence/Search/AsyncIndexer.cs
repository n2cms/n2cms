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

        Queue<Action> errorQueue = new Queue<Action>();
        DateTime lastAttempt = DateTime.Now;
        bool async;
        bool handleErrors;
        int workerCount;
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


        protected void DoWork(Action action)
        {
            if (handleErrors)
                action = WrapActionInErrorHandling(action);

            if (async)
                worker.DoWork(() =>
                {
                    Interlocked.Increment(ref workerCount);
                    try
                    {
                        action();
                        persister.Dispose();
                    }
                    finally
                    {
                        Interlocked.Decrement(ref workerCount);
                        currentWork = "";
                    }
                });
            else
                action();
        }

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

        public virtual void ReindexDescendants(ContentItem root, bool clearBeforeReindex)
        {
            DoWork(() =>
                {
                    if(clearBeforeReindex)
                        indexer.Delete(root.ID);

                    root = persister.Get(root.ID);
                    indexer.Update(root);
                    ReindexChildren(root.ID);
                });
        }

        private void ReindexChildren(int parentID)
        {
            var childrenIds = new List<int>();
            
            using (persister)
            {
                foreach (var child in persister.Repository.Find("Parent.ID", parentID))
                {
                    if(child.AlteredPermissions == Security.Permission.None)
                        currentWork = "Indexing " + child.Title + " #" + child.ID;
                    else
                        currentWork = "Indexing item #" + child.ID;
                    
                    indexer.Update(child);
                    childrenIds.Add(child.ID);
                }
            }

            foreach (var id in childrenIds)
                ReindexChildren(id);
        }

        public virtual void Reindex(int itemID, bool affectsChildren)
        {
            DoWork(() =>
            {
                var item = persister.Get(itemID);
                if (item == null)
                    return;

                currentWork = "Indexing " + item.Title + " #" + itemID;
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

        public virtual void Delete(int itemID)
        {
            DoWork(() =>
            {
                currentWork = "Deleting #" + itemID;
                indexer.Delete(itemID);
            });
        }

        public IndexStatus GetCurrentStatus()
        {
            return new IndexStatus
            {
                CurrentWork = currentWork,
                WorkerCount = workerCount,
                ErrorQueueCount = errorQueue.Count
            };
        }
    }
}
