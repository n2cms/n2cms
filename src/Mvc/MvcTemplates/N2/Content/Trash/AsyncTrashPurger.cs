using N2.Edit.Trash;
using N2.Engine;
using N2.Persistence;
using N2.Security;
using System;
using System.Collections.Concurrent;

namespace N2.Management.Content.Trash
{
    public class AsyncPurgeStatus
    {
        public string Title { get; set; }
        public bool IsRunning { get; set; }
        public PurgingStatus Progress { get; set; }
    }

    public class Work
    {
        public Action Task { get; set; }
    }

    [Service]
    public class AsyncTrashPurger
    {
        private IWorker worker;
        private ITrashHandler trash;
        private IPersister persister;

        private AsyncPurgeStatus _currentStatus;
        bool _isWorking;
        ConcurrentQueue<Work> workQueue = new ConcurrentQueue<Work>();
        private ISecurityManager security;

        public AsyncTrashPurger(IWorker worker, ITrashHandler trash, IPersister persister, ISecurityManager security)
        {
            this.worker = worker;
            this.trash = trash;
            this.persister = persister;
            this.security = security;
        }

        public AsyncPurgeStatus Status
        {
            get { return _currentStatus ?? new AsyncPurgeStatus { IsRunning = false }; }
            set { _currentStatus = value; }
        }

        public virtual void BeginPurgeAll()
        {
            workQueue.Enqueue(new Work { Task = () => 
            {
                Status = new AsyncPurgeStatus { IsRunning = true, Progress = new PurgingStatus { Deleted = 0, Remaining = 1 }, Title = "All" };
                using (security.Disable())
                {
                    trash.PurgeAll(s => { Status = new AsyncPurgeStatus { IsRunning = true, Progress = s, Title = "All" }; });
                }
                Status = null;
            }});
            BeginWorking();
        }

        public virtual void BeginPurge(int rootId)
        {
            workQueue.Enqueue(new Work
            {
                Task = () =>
                {
                    var item = persister.Get(rootId);
                    if (item == null)
                        return;

                    string title = item.Title;
                    Status = new AsyncPurgeStatus { IsRunning = true, Progress = new PurgingStatus { Deleted = 0, Remaining = 1 }, Title = title };

                    using(this.security.Disable())
                    {
                        trash.Purge(item, s => { Status = new AsyncPurgeStatus { IsRunning = true, Progress = s, Title = title }; });
                    }
                    Status = null;
                }
            });
            BeginWorking();
        }

        private void BeginWorking()
        {
            lock (this)
            {
                if (_isWorking)
                    return;

                Status = new AsyncPurgeStatus { IsRunning = true };
                _isWorking = true;
                worker.DoWork(() =>
                    {
                        Work work;
                        while (workQueue.TryDequeue(out work))
                        {
                            work.Task();
                        }

                        lock (this)
                        {
                            _isWorking = false;
                        }
                    });
            }
        }
    }
}
