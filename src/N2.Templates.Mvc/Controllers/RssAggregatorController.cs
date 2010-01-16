using System;
using System.Collections.Generic;
using System.Security;
using System.Web.Mvc;
using System.Xml;
using System.Xml.XPath;
using N2.Templates.Mvc.Models.Parts;
using N2.Templates.Mvc.Models;
using N2.Web;

namespace N2.Templates.Mvc.Controllers
{
	[Controls(typeof (RssAggregator))]
	public class RssAggregatorController : TemplatesControllerBase<RssAggregator>
	{
		private const int ExpirationTime = 1;

		[OutputCache(Duration = ExpirationTime, VaryByParam = "*")]
		public override ActionResult Index()
		{
			return View(new RssAggregatorModel(CurrentItem, GetNewsItems(CurrentItem.RssUrl)));
		}

		private IEnumerable<RssAggregatorModel.RssItem> GetNewsItems(string url)
		{
			try
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
			catch(SecurityException)
			{
				// Cannot use this in Medium Trust
				return new RssAggregatorModel.RssItem[]
				       	{
				       		new RssAggregatorModel.RssItem()
				       			{
				       				Title = "Cannot load RSS Feed",
									Url = "#",
									Introduction = "Could not load RSS feed because security settings would not allow it",
				       			}, 
			};
			}
		}

		private static string GetValue(XPathNavigator item, string xpath)
		{
			XPathNavigator node = item.SelectSingleNode(xpath);
			return node != null ? node.Value : null;
		}
	}
}