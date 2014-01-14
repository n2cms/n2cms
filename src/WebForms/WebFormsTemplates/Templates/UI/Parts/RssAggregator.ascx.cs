using System;
using System.Web.Caching;
using N2;
using System.Xml.XPath;
using System.Xml;
using System.Collections.Generic;
using N2.Engine;
using N2.Web;
using N2.Web.UI;

namespace N2.Templates.UI.Parts
{
    public partial class RssAggregator : Web.UI.TemplateUserControl<ContentItem, Templates.Items.RssAggregator>
    {
        private class RssItem
        {
            public string Title { get; set; }
            public string Introduction { get; set; }
            public string Published { get; set; }
            public string Url { get; set; }
        }

        protected override void OnInit(EventArgs e)
        {
            string url = CurrentItem.RssUrl;
            if (!string.IsNullOrEmpty(url))
            {
                rptRss.DataSource = CacheNewsItems(url, GetNewsItems);
                rptRss.DataBind();
            }
            base.OnInit(e);
        }

        private string CacheKey
        {
            get { return "RssAggregator.NewsItems" + CurrentItem.ID; }
        }

        private static TimeSpan ExpirationTime = TimeSpan.FromMinutes(1);
        private IEnumerable<RssItem> CacheNewsItems(string url, Func<string, IEnumerable<RssItem>> reader)
        {
            IEnumerable<RssItem> items = Cache[CacheKey] as IEnumerable<RssItem>;
            if(items == null)
            {
                try
                {
                    items = new List<RssItem>(reader(url));   
                }
                catch (Exception ex)
                {
                    items = new RssItem[0];
                    Engine.Resolve<IErrorNotifier>().Notify(ex);
                }
                Cache.Add(CacheKey,
                          items,
                          new ContentCacheDependency(Engine.Persister),
                          N2.Utility.CurrentTime().Add(ExpirationTime),
                          Cache.NoSlidingExpiration,
                          CacheItemPriority.Normal, null);
            }
            return items;
        }

        private IEnumerable<RssItem> GetNewsItems(string url)
        {
            List<RssItem> news = new List<RssItem>();
            using (XmlReader reader = XmlReader.Create(url))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(reader);
                XPathNavigator navigator = doc.CreateNavigator();
                foreach (XPathNavigator item in navigator.Select("//item"))
                {
                    string title = GetValue(item, "title");
                    string link = GetValue(item, "link");
                    string description = GetValue(item, "description");
                    string pubDate = GetValue(item, "pubDate");
                    RssItem rss = new RssItem
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
