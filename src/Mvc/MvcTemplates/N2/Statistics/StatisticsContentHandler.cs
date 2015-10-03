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

		protected override object HandleDataRequest(HttpContextBase context)
		{
			DateTime from, to;
			if (!DateTime.TryParse(context.Request["from"], out to))
				to = Utility.CurrentTime().GetSlot(granularity).Add(slotSize);
			if (!DateTime.TryParse(context.Request["from"], out from))
				from = to.AddDays(-displayedDays);
			
			int id;
			if (!int.TryParse(context.Request.QueryString["n2item"], out id))
				id = 0;

			bool raw;
			if (!bool.TryParse(context.Request.QueryString["raw"], out raw))
				raw = false;

			var statistics = repository.GetStatistics(from, to, id)
				//.GroupBy(s => new { s.PageID, Day = s.TimeSlot.Date })
				//.Select(g => new Statistic { PageID = g.Key.PageID, TimeSlot = g.Key.Day, Views = g.Sum(s => s.Views) })
				//.OrderBy(s => s.TimeSlot)
				.ToList();

			if (raw)
				return new { Views = statistics };

			var views = Slottify(statistics, from, to).ToList();
			if (id == 0)
			{
				var pages = statistics.GroupBy(s => s.PageID)
					.OrderByDescending(g => g.Sum(x => x.Views))
					.Select(g =>
					{
						var slots = Slottify(g, from, to).ToList();
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