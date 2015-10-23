using N2.Engine;
using N2.Persistence;
using N2.Persistence.NH;
using N2.Plugin.Scheduling;
using NHibernate.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Management.Statistics
{
	[Service(typeof(StatisticsRepository), Replaces = typeof(StatisticsRepository), Configuration = "sql")]
	public class SqlBucketRepository : StatisticsRepository
	{
		private ISessionProvider session;
		public static DateTime? AdoExceptionDetected { get; set; }

		public SqlBucketRepository(N2.Persistence.IRepository<Bucket> buckets, N2.Persistence.IRepository<Statistic> statistics, ISessionProvider session)
			: base(buckets, statistics)
		{
			this.session = session;
		}

		public override IEnumerable<Statistic> GetStatistics(DateTime from, DateTime to, params int[] itemIds)
		{
			try
			{
				var result = base.GetStatistics(from, to, itemIds);
				AdoExceptionDetected = null;
				return result;
			}
			catch (GenericADOException)
			{
				MarkAdoException();
				throw;
			} 
		}

		public override void Save(IEnumerable<Bucket> buckets)
		{
			try
			{
				base.Save(buckets);
				AdoExceptionDetected = null;
			}
			catch (GenericADOException)
			{
				MarkAdoException();
				throw;
			} 
		}

		public override void Transfer(DateTime uptil, Granularity interval)
		{
			try
			{
				base.Transfer(uptil, interval);
				AdoExceptionDetected = null;
			}
			catch (GenericADOException)
			{
				MarkAdoException();
				throw;
			}
		}

		private void MarkAdoException()
		{
			if (!AdoExceptionDetected.HasValue)
				AdoExceptionDetected = Utility.CurrentTime();
		}
	}

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
			using (var tx = buckets.BeginTransaction())
			{
				var slotEndTime = uptil.GetSlot(interval, endOfSlot: true);
				var collectedBuckets = buckets.Find().Where(b => b.TimeSlot < slotEndTime).OrderBy(b => b.TimeSlot).ToArray();
				if (collectedBuckets.Length == 0)
					return;

				ClearBuckets(collectedBuckets);

				var start = collectedBuckets[0].TimeSlot;
				var pageViews = collectedBuckets
					.GroupBy(b => new KeyValuePair<DateTime, int>(b.TimeSlot.GetSlot(interval), b.PageID), b => b.Views)
					.ToDictionary(b => b.Key, b => b.Sum());

				var existingStatistics = IncrementExistingStatistics(uptil, interval, start, pageViews);

				var addedStatistics = InsertNewStatistics(pageViews);
				
				tx.Commit();

				logger.InfoFormat("Transferred {0} buckets into {1} new and {2} updated statistics", collectedBuckets.Length, addedStatistics.Count, existingStatistics.Count);
			}
		}

		private List<Statistic> InsertNewStatistics(Dictionary<KeyValuePair<DateTime, int>, int> pageViews)
		{
			var addedStatistics = new List<Statistic>(pageViews.Count);
			foreach (var pageView in pageViews)
			{
				var s = new Statistic { TimeSlot = pageView.Key.Key, PageID = pageView.Key.Value, Views = pageView.Value };
				addedStatistics.Add(s);
			}
			statistics.SaveOrUpdate(addedStatistics);
			statistics.Flush();
			return addedStatistics;
		}

		private List<Statistic> IncrementExistingStatistics(DateTime uptil, Granularity interval, DateTime start, Dictionary<KeyValuePair<DateTime, int>, int> pageViews)
		{
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
			return existingStatistics;
		}

		private void ClearBuckets(Bucket[] collectedBuckets)
		{
			buckets.Delete(collectedBuckets);
			buckets.Flush();
		}

		public virtual IEnumerable<Statistic> GetStatistics(DateTime from, DateTime to, params int[] itemIds)
		{
			var p = Parameter.GreaterOrEqual("TimeSlot", from) & Parameter.LessThan("TimeSlot", to);
			if (itemIds != null)
			{
				if (itemIds.Length == 1)
					p = p & Parameter.Equal("PageID", itemIds[0]);
				else if (itemIds.Length > 1)
					p = p & Parameter.In("PageID", itemIds.OfType<object>().ToArray());
			}
			p = p.OrderBy("TimeSlot");
			var data = this.statistics.Find(p).ToList();
			return data;
		}

		public virtual int Delete(DateTime? from, DateTime? to, int? id)
		{
			var itemsToRemove = GetStatistics(from ?? new DateTime(2000, 1, 1), to ?? new DateTime(2100, 1, 1), id.HasValue ? new [] { id.Value } : new int[0]).ToArray();
			this.statistics.Delete(itemsToRemove);
			this.statistics.Flush();
			return itemsToRemove.Length;
		}
	}
}
