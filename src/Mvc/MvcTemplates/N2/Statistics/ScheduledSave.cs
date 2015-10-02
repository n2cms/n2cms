using N2.Engine;
using N2.Persistence;
using N2.Persistence.NH;
using N2.Plugin.Scheduling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace N2.Management.Statistics
{
	internal static class StatisticsExtension
	{
		public static DateTime GetSlot(this DateTime date, TimeUnit interval)
		{
			if (interval == TimeUnit.Seconds)
				return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
			else if (interval == TimeUnit.Minutes)
				return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, 0);
			else if (interval == TimeUnit.Hours)
				return new DateTime(date.Year, date.Month, date.Day, date.Hour, 0, 0);
			else
				return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
		}
	}

	[Service(typeof(StatisticsRepository), Replaces = typeof(StatisticsRepository), Configuration = "sql")]
	public class SqlBucketRepository : StatisticsRepository
	{
		private ISessionProvider session;

		public SqlBucketRepository(N2.Persistence.IRepository<Bucket> buckets, N2.Persistence.IRepository<Statistic> statistics, ISessionProvider session)
			: base(buckets, statistics)
		{
			this.session = session;
		}
	}

	[ScheduleExecution(1, TimeUnit.Minutes)]
	public class ScheduledSave : ScheduledAction
	{
		private Collector filler;
		private StatisticsRepository repository;
		public TimeUnit CheckoutInterval { get; set; }
		public TimeUnit StatisticsGranularity { get; set; }

		public ScheduledSave(Collector filler, StatisticsRepository repository)
		{
			CheckoutInterval = TimeUnit.Minutes;
			StatisticsGranularity = TimeUnit.Minutes;
			this.filler = filler;
			this.repository = repository;
		}

		public override void Execute()
		{
			var now = Utility.CurrentTime();

			if (LastExecuted.HasValue)
			{
				if (LastExecuted.Value.GetSlot(CheckoutInterval) != now.GetSlot(CheckoutInterval))
				{
					var buckets = filler.CheckoutBuckets();
					repository.Save(buckets);
				}
			}

			if (!LastExecuted.HasValue || LastExecuted.Value.GetSlot(StatisticsGranularity) != now.GetSlot(StatisticsGranularity))
			{
				repository.Transfer(now, StatisticsGranularity);
			}
		}
	}
}