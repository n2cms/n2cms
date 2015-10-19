using N2.Definitions;
using N2.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace N2.Management.Statistics
{
	[Service]
	public class PopularityChildrenSorter : IChildrenSorter
	{
		private StatisticsRepository statistics;

		public PopularityChildrenSorter(StatisticsRepository statistics)
		{
			this.statistics = statistics;
			NumberOfDaysToConsider = 7;
		}

		public IEnumerable<ContentItem> ReorderChildren(ContentItem item)
		{
			var childPages = item.Children.FindPages().ToList();
			var stats = statistics.GetStatistics(Utility.CurrentTime().Subtract(TimeSpan.FromDays(NumberOfDaysToConsider)), Utility.CurrentTime(), childPages.Select(p => p.ID).ToArray());
			var statsDictionary = stats.GroupBy(s => s.PageID, s => s.Views).ToDictionary(s => s.Key, s => s.Sum());
			foreach(var page in childPages)
			{
				var newOrder = statsDictionary.ContainsKey(page.ID) 
					? -statsDictionary[page.ID]
					: 0;
				if (newOrder == page.SortOrder)
					continue;
				page.SortOrder = newOrder;
				yield return page;
			}
		}

		public int NumberOfDaysToConsider { get; set; }
	}
}