using System.Collections.Generic;
using MvcTest.Models;

namespace MvcTest.Views.NewsContainer
{
	public class NewsContainerViewData
	{
		public Models.NewsContainer Container { get; set; }
		public IEnumerable<NewsPage> News { get; set; }
	}
}
