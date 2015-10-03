using N2.Engine;
using N2.Persistence;
using N2.Plugin.Scheduling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Management.Statistics
{
	//[Service(typeof(StatisticsRepository), Replaces = typeof(StatisticsRepository), Configuration = "sql")]
	//public class SqlBucketRepository : StatisticsRepository
	//{
	//	private ISessionProvider session;

	//	public SqlBucketRepository(N2.Persistence.IRepository<Bucket> buckets, N2.Persistence.IRepository<Statistic> statistics, ISessionProvider session)
	//		: base(buckets, statistics)
	//	{
	//		this.session = session;
	//	}
	//}

	[Service]
	public class StatisticsRepository
	{
		private Persistence.IRepository<Bucket> buckets;
		private Persistence.IRepository<Statistic> statistics;
		Logger<StatisticsRepository> logger;

		public StatisticsRepository(N2.Persistence.IRepository<Bucket> buckets, N2.Persistence.IRepository<Statistic> statistics)
		{
			this.buckets = buckets;
			this.statistics = statistics;
		}

		public virtual void Save(IEnumerable<Bucket> buckets)
		{
			int count = 0;
			foreach (var bucket in buckets)
			{
				this.buckets.SaveOrUpdate(bucket);
				count++;
			}
			if (count > 0)
				this.buckets.Flush();

			logger.InfoFormat("Saved {0} statistics buckets", count);
		}

		public virtual void Transfer(DateTime uptil, Granularity interval)
		{
			var slot = uptil.GetSlot(interval);
			var collectedBuckets = buckets.Find().Where(b => b.TimeSlot < slot).ToArray();
			if (collectedBuckets.Length == 0)
				return;

			var start = collectedBuckets[0].TimeSlot;
			var pageViews = collectedBuckets
				.GroupBy(b => new KeyValuePair<DateTime, int>(b.TimeSlot.GetSlot(interval), b.PageID), b => b.Views)
				.ToDictionary(b => b.Key, b => b.Sum());

			var existingStatistics = statistics.Find(Parameter.GreaterOrEqual("TimeSlot", start) & Parameter.LessThan("TimeSlot", uptil)).ToList();
			foreach (var s in existingStatistics)
			{
				var key = new KeyValuePair<DateTime, int>(s.TimeSlot.GetSlot(interval), s.PageID);
				if (!pageViews.ContainsKey(key))
					continue;
				s.Views += pageViews[key];
				pageViews.Remove(key);
			}

			statistics.SaveOrUpdate(existingStatistics);
			statistics.Flush();
			buckets.Delete(collectedBuckets);
			buckets.Flush();

			var addedStatistics = new List<Statistic>(pageViews.Count);
			foreach (var pageView in pageViews)
			{
				var s = new Statistic { TimeSlot = pageView.Key.Key, PageID = pageView.Key.Value, Views = pageView.Value };
				addedStatistics.Add(s);
			}
			statistics.SaveOrUpdate(addedStatistics);
			statistics.Flush();

			logger.InfoFormat("Transferred {0} buckets into {1} new and {2} updated statistics", collectedBuckets.Length, addedStatistics.Count, existingStatistics.Count);
		}

		public IEnumerable<Statistic> GetStatistics(DateTime from, DateTime to, int id = 0)
		{
			var p = Parameter.GreaterOrEqual("TimeSlot", from) & Parameter.LessThan("TimeSlot", to);
			if (id != 0)
				p = p & Parameter.Equal("PageID", id);
			p = p.OrderBy("TimeSlot");
			var data = this.statistics.Find(p).ToList();
			return data;
		}
	}
}
