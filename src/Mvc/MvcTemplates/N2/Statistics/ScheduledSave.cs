using N2.Engine;
using N2.Plugin.Scheduling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace N2.Management.Statistics
{
	[Service]
	public class BucketRepository
	{
		private Persistence.IRepository<Bucket> repository;
			
		public BucketRepository(N2.Persistence.IRepository<Bucket> repository)
		{
			this.repository = repository;
		}
		
		public void Save(IEnumerable<Bucket> buckets)
		{
			foreach (var bucket in buckets)
			{
				repository.SaveOrUpdate(bucket);
			}
			repository.Flush();
		}
	}

	[ScheduleExecution(1, TimeUnit.Minutes)]
	public class ScheduledSave : ScheduledAction
	{
		private BucketFiller filler;
		private BucketRepository repository;

		public ScheduledSave(BucketFiller filler, BucketRepository repository)
		{
			this.filler = filler;
			this.repository = repository;
		}

		public override void Execute()
		{
			if (!LastExecuted.HasValue || LastExecuted.Value.Minute == Utility.CurrentTime().Minute)
				return;

			var buckets = filler.CheckoutBuckets();
			repository.Save(buckets);
		}
	}
}