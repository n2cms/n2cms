using System;
using System.Collections.Generic;
using N2.Templates.Mvc.Models.Parts;

namespace N2.Templates.Mvc.Models
{
	public class RssAggregatorModel
	{
		public RssAggregator Item { get; set; }
		public IEnumerable<RssItem> Items { get; set; }

		public RssAggregatorModel(RssAggregator item, IEnumerable<RssItem> items)
		{
			Item = item;
			Items = items;
		}

		public class RssItem
		{
			public string Title { get; set; }
			public string Introduction { get; set; }
			public string Published { get; set; }
			public string Url { get; set; }
		}
	}
}