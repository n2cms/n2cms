using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Xml;
using System.Xml.XPath;
using N2.Templates.Mvc.Items.Items;
using N2.Templates.Mvc.Models;
using N2.Web;

namespace N2.Templates.Mvc.Controllers
{
	[Controls(typeof (RssAggregator))]
	public class RssAggregatorController : N2Controller<RssAggregator>
	{
		private const string CacheKey = "RssAggregator.NewsItems";
		private const int ExpirationTime = 1;

		[OutputCache(Duration = ExpirationTime, VaryByParam = "*")]
		public override ActionResult Index()
		{
			return View(GetTemplateUrl(), new RssAggregatorModel(CurrentItem, GetNewsItems(CurrentItem.RssUrl)));
		}

		private IEnumerable<RssAggregatorModel.RssItem> GetNewsItems(string url)
		{
			var news = new List<RssAggregatorModel.RssItem>();
			using (XmlReader reader = XmlReader.Create(url))
			{
				var doc = new XmlDocument();
				doc.Load(reader);
				XPathNavigator navigator = doc.CreateNavigator();
				foreach (XPathNavigator item in navigator.Select("//item"))
				{
					string title = GetValue(item, "title");
					string link = GetValue(item, "link");
					string description = GetValue(item, "description");
					string pubDate = GetValue(item, "pubDate");
					var rss = new RssAggregatorModel.RssItem
					          	{
					          		Title = title,
					          		Url = link,
					          		Introduction = description,
					          		Published = pubDate
					          	};

					news.Add(rss);
					if (news.Count >= CurrentItem.MaxCount)
						break;
				}
			}
			return news;
		}

		private static string GetValue(XPathNavigator item, string xpath)
		{
			XPathNavigator node = item.SelectSingleNode(xpath);
			return node != null ? node.Value : null;
		}
	}
}