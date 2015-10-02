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

		public StatisticsContentHandler(StatisticsRepository repository)
		{
			this.repository = repository;
		}

		protected override object HandleDataRequest(HttpContextBase context)
		{
			DateTime from, to;
			if (!DateTime.TryParse(context.Request["from"], out from))
				from = Utility.CurrentTime().AddMonths(-1).Date;
			if (!DateTime.TryParse(context.Request["from"], out to))
				to = Utility.CurrentTime().Date.AddDays(1);

			int id;
			if (!int.TryParse(context.Request.QueryString["id"], out id))
				id = 0;

			bool process;
			if (!bool.TryParse(context.Request.QueryString["process"], out process))
				process = true;

			var statistics = repository.GetStatistics(from, to, id)
				//.GroupBy(s => new { s.PageID, Day = s.TimeSlot.Date })
				//.Select(g => new Statistic { PageID = g.Key.PageID, TimeSlot = g.Key.Day, Views = g.Sum(s => s.Views) })
				//.OrderBy(s => s.TimeSlot)
				.ToList();

			if (process)
			{
				if (id == 0)
				{
					var slots = Slottify(statistics, from, to).ToList();
					return new
					{
						Max = slots.Max(s => s.Views),
						Views = slots
					};
				}
				else
					return new
					{
						Pages = statistics.GroupBy(s => s.PageID)
							.OrderByDescending(g => g.Sum(x => x.Views))
							.Select(g =>
							{
								var slots = Slottify(g, from, to).ToList();
								return new { PageID = g.Key, TotalViews = g.Sum(x => x.Views), Max = slots.Max(v => v.Views), Views = slots };
							})
					};
			}
			else
				return new { Views = statistics };
		}

		public class Stat
		{
			public DateTime Day { get; set; }
			public int Views { get; set; }
		}

		private IEnumerable<Stat> Slottify(IEnumerable<Statistic> values, DateTime from, DateTime to)
		{
			var days = Enumerable.Range(0, (int)to.Subtract(from).TotalDays).Select(d => from.AddDays(d)).ToList();

			var valueEnumerator = values.GetEnumerator();
			bool hasNext = valueEnumerator.MoveNext();
			foreach(var day in days)
			{
				if (!hasNext)
				{
					yield return new Stat { Day = day };
					continue;
				}

				if (valueEnumerator.Current.TimeSlot < day.AddDays(1))
				{
					int views = valueEnumerator.Current.Views;
					while (hasNext = valueEnumerator.MoveNext() && valueEnumerator.Current.TimeSlot < day.AddDays(1))
						views += valueEnumerator.Current.Views;
					yield return new Stat { Day = day, Views = views };
				}
				else
					yield return new Stat { Day = day };
			}
		}
	}
}