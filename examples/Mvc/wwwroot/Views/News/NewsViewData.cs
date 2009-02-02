using MvcTest.Models;
using System.Collections.Generic;
using N2;

namespace MvcTest.Views.News
{
	public class NewsViewData
	{
		public ContentItem Back { get; set; }
		public NewsPage News { get; set; }
		public IEnumerable<CommentItem> Comments { get; set; }
	}
}
