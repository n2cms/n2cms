using N2.Configuration;
using N2.Edit.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace N2.Management.Statistics
{
	[ContentHandler]
	public class StatisticsContentHandler : ContentHandlerBase
	{
		private StatisticsRepository repository;
		private int displayedDays;
		private Granularity granularity;
		private TimeSpan slotSize;

		public StatisticsContentHandler(StatisticsRepository repository, ConfigurationManagerWrapper config)
		{
			this.repository = repository;
			var section = config.GetContentSection<Configuration.StatisticsSection>("statistics", required: false);
			this.displayedDays = section.DisplayedDays;
			this.granularity = section.Granularity;
			this.slotSize = granularity == Granularity.Day
				? TimeSpan.FromDays(1)
				: granularity == Granularity.Hour
					? TimeSpan.FromHours(1)
					: TimeSpan.FromMinutes(1);
		}

		public object DeleteIndex(DateTime? from, DateTime? to, int? n2item)
		{
			return new { RemovedCount = repository.Delete(from, to, n2item) };
		}

		public object Index(DateTime? from, DateTime? to, int? n2item, bool? raw)
		{
			to = to ?? Utility.CurrentTime().GetSlot(granularity).Add(slotSize);
			from = from ?? to.Value.AddDays(-displayedDays);

			var statistics = repository.GetStatistics(from.Value, to.Value, n2item ?? 0)
				.ToList();

			if (raw.HasValue && raw.Value)
				return new { Views = statistics };

			var views = Slottify(statistics, from.Value, to.Value).ToList();
			if (n2item == 0)
			{
				var pages = statistics.GroupBy(s => s.PageID)
					.OrderByDescending(g => g.Sum(x => x.Views))
					.Select(g =>
					{
						var slots = Slottify(g, from.Value, to.Value).ToList();
						return new { PageID = g.Key, TotalViews = g.Sum(x => x.Views), Max = slots.Max(v => v.Views), Views = slots };
					}).ToList();
				return new
				{
					Max = views.Select(s => s.Views).DefaultIfEmpty().Max(),
					Views = views,
					Pages = pages
				};
			}
			else
			{
				return new
				{
					Max = views.Select(s => s.Views).DefaultIfEmpty().Max(),
					Views = views
				};
			}
		}

		public class Slot
		{
			public DateTime Date { get; set; }
			public int Views { get; set; }
		}

		private IEnumerable<Slot> Slottify(IEnumerable<Statistic> values, DateTime from, DateTime to)
		{
			int numberOfDays = (int)to.Subtract(from).TotalDays;
			List<DateTime> slots;
			if (granularity == Granularity.Day)
				slots = Enumerable.Range(0, numberOfDays).Select(d => from.AddDays(d)).ToList();
			else if (granularity == Granularity.Hour)
				slots = Enumerable.Range(0, numberOfDays * 24).Select(h => from.AddHours(h)).ToList();
			else
				slots = Enumerable.Range(0, numberOfDays * 24 * 60).Select(h => from.AddMinutes(h)).ToList();
			

			var valueEnumerator = values.GetEnumerator();
			bool hasNext = valueEnumerator.MoveNext();
			foreach(var slot in slots)
			{
				if (valueEnumerator.Current != null && valueEnumerator.Current.TimeSlot < slot.Add(slotSize))
				{
					int views = valueEnumerator.Current.Views;
					while (hasNext = valueEnumerator.MoveNext())
					{
						if (valueEnumerator.Current.TimeSlot < slot.Add(slotSize))
							views += valueEnumerator.Current.Views;
						else
							break;
					}
					yield return new Slot { Date = slot, Views = views };
				}
				else
					yield return new Slot { Date = slot };
			}
		}
	}
}