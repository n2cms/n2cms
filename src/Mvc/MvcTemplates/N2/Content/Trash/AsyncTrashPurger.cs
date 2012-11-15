using N2;
using N2.Edit.Trash;
using N2.Engine;
using N2.Persistence;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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

		private AsyncPurgeStatus currentStatus;
		bool isWorking;
		ConcurrentQueue<Work> workQueue = new ConcurrentQueue<Work>();

		public AsyncTrashPurger(IWorker worker, ITrashHandler trash, IPersister persister)
		{
			this.worker = worker;
			this.trash = trash;
			this.persister = persister;
		}

		public AsyncPurgeStatus Status
		{
			get { return currentStatus ?? new AsyncPurgeStatus { IsRunning = false }; }
			set { currentStatus = value; }
		}

		public virtual void BeginPurgeAll()
		{
			workQueue.Enqueue(new Work { Task = () => 
			{
				Status = new AsyncPurgeStatus { IsRunning = true, Progress = new PurgingStatus { Deleted = 0, Remaining = 1 }, Title = "All" };
				trash.PurgeAll((s) => { Status = new AsyncPurgeStatus { IsRunning = true, Progress = s, Title = "All" }; });
				Status = null;
			}});
			BeginWorking();
		}

		public virtual void BeginPurge(int rootID)
		{
			workQueue.Enqueue(new Work
			{
				Task = () =>
				{
					var item = persister.Get(rootID);
					if (item == null)
						return;

					string title = item.Title;
					Status = new AsyncPurgeStatus { IsRunning = true, Progress = new PurgingStatus { Deleted = 0, Remaining = 1 }, Title = title };

					trash.Purge(item, (s) => { Status = new AsyncPurgeStatus { IsRunning = true, Progress = s, Title = title }; });
					Status = null;
				}
			});
			BeginWorking();
		}

		private void BeginWorking()
		{
			lock (this)
			{
				if (isWorking)
					return;

				Status = new AsyncPurgeStatus { IsRunning = true };
				isWorking = true;
				worker.DoWork(() =>
					{
						Work work;
						while (workQueue.TryDequeue(out work))
						{
							work.Task();
						}

						lock (this)
						{
							isWorking = false;
						}
					});
			}
		}
	}
}