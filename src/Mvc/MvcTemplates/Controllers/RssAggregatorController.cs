using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.ServiceModel.Syndication;
using System.Web.Caching;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using N2.Templates.Mvc.Models;
using N2.Templates.Mvc.Models.Parts;
using N2.Web;
using N2.Web.UI;
using System.Net;
using System.Net.Sockets;

namespace N2.Templates.Mvc.Controllers
{
	[Controls(typeof(RssAggregator))]
	public class RssAggregatorController : TemplatesControllerBase<RssAggregator>
	{
		private const int ExpirationTime = 1;
		private readonly Engine.Logger<RssAggregatorController> logger;

		public override ActionResult Index()
		{
			RssAggregatorModel.RssItem[] items = GetItemsFromCache();
			if (items != null)
				return PartialView("List", new RssAggregatorModel(CurrentItem, items));

			return PartialView(CurrentItem);
		}

		public ActionResult List()
		{
			RssAggregatorModel.RssItem[] items = GetItemsFromAllHosts();
			return PartialView(new RssAggregatorModel(CurrentItem, items));
		}

		private RssAggregatorModel.RssItem[] GetItemsFromAllHosts()
		{
			RssAggregatorModel.RssItem[] items = GetItemsFromCache();
			if (items == null)
			{
				items = CurrentItem.RssUrls
					.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
					.SelectMany(u => GetNewsItems(u))
					.Distinct()
					.OrderBy(ri => ri.Published)
					.Take(CurrentItem.MaxCount)
					.ToArray();

				HttpContext.Cache.Add(CacheKey,
					items,
					new ContentCacheDependency(Engine.Persister),
					DateTime.Now.AddSeconds(60),
					TimeSpan.Zero,
					CacheItemPriority.Normal,
					null);
			}
			return items;
		}

		private RssAggregatorModel.RssItem[] GetItemsFromCache()
		{
			return HttpContext.Cache[CacheKey] as RssAggregatorModel.RssItem[];
		}

		private string CacheKey
		{
			get { return "RssAggregator" + CurrentItem.ID; }
		}

		private IEnumerable<RssAggregatorModel.RssItem> GetNewsItems(string url)
		{
			try
			{
				var xml = XDocument.Load(url).ToString();
				
				foreach (var formatter in new SyndicationFeedFormatter[] { new Rss20FeedFormatter(), new Atom10FeedFormatter() })
				{
					using (var reader = new XmlTextReader(new StringReader(xml)))
					{
						if(!formatter.CanRead(reader))
							continue;

						formatter.ReadFrom(reader);
						return formatter.Feed.Items.Select(i => new RssAggregatorModel.RssItem
						{
							Title = i.Title.Text,
							Introduction = i.Summary != null ? i.Summary.Text : "",
							Published = i.PublishDate,
							Url = i.Links.Where(l => l.MediaType == "text/html").Select(l => l.Uri.ToString()).FirstOrDefault() 
								?? i.Links.Select(l => l.Uri.ToString()).FirstOrDefault()
						});
					}
				}
			}
			catch (SecurityException)
			{
				// Cannot use this in Medium Trust
                return GetCannotLoadItem(Resources.RssAggregator.CannotLoadCauseSecurity);
			}
			catch (SocketException)
			{
				//Feed Is Offline or inaccessible
				return GetCannotLoadItem("Could not load RSS feed due to network connectivity failure.");
			}
			catch (WebException)
			{
				//invalid feed address?
				return GetCannotLoadItem("Could not load RSS feed, possible failure resolving remote host.");
			}
			catch(Exception ex)
			{
				logger.Error(ex);
			}
			return GetCannotLoadItem("");
		}

		private static IEnumerable<RssAggregatorModel.RssItem> GetCannotLoadItem(string reason)
		{
			return new RssAggregatorModel.RssItem[]
				{
				    new RssAggregatorModel.RssItem()
				       	{
				       		Title = Resources.RssAggregator.CannotLoadRss,
							Published = DateTime.Now,
							Url = "#",
							Introduction = reason,
				       	}, 
				};
		}

		private static string GetValue(XPathNavigator item, string xpath)
		{
			XPathNavigator node = item.SelectSingleNode(xpath);
			return node != null ? node.Value : null;
		}
	}
}