using N2.Configuration;
using N2.Engine;
using N2.Management.Statistics.Configuration;
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
		public static DateTime GetSlot(this DateTime date, Granularity interval)
		{
			if (interval == Granularity.Minute)
				return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, 0);
			else if (interval == Granularity.Hour)
				return new DateTime(date.Year, date.Month, date.Day, date.Hour, 0, 0);
			else
				return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
		}
	}

	[ScheduleExecution(1, TimeUnit.Minutes)]
	public class ScheduledSave : ScheduledAction
	{
		private Collector filler;
		private StatisticsRepository repository;
		public Granularity MemoryFlushInterval { get; set; }
		public Granularity StatisticsGranularity { get; set; }
		public Granularity TransferInterval { get; set; }

		public ScheduledSave(Collector filler, StatisticsRepository repository, ConfigurationManagerWrapper config)
		{
			var section = config.GetContentSection<StatisticsSection>("statistics", required: false);
			MemoryFlushInterval = section.MemoryFlushInterval;
			TransferInterval = section.TransferInterval;
			StatisticsGranularity = section.Granularity;
			this.filler = filler;
			this.repository = repository;
		}

		public override void Execute()
		{
			var now = Utility.CurrentTime();

			if (LastExecuted.HasValue)
			{
				if (LastExecuted.Value.GetSlot(MemoryFlushInterval) != now.GetSlot(MemoryFlushInterval))
				{
					var buckets = filler.CheckoutBuckets();
					repository.Save(buckets);
				}
			}

			if (!LastExecuted.HasValue || LastExecuted.Value.GetSlot(TransferInterval) != now.GetSlot(TransferInterval))
			{
				repository.Transfer(now, StatisticsGranularity);
			}
		}

	}
}