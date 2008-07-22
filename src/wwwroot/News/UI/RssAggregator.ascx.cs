using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2;
using System.Xml.XPath;
using System.Xml;
using System.Collections.Generic;

namespace N2.Templates.News.UI
{
    public partial class RssAggregator : Web.UI.TemplateUserControl<ContentItem, Items.RssAggregator>
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
                rptRss.DataSource = GetNewsItems(url);
                rptRss.DataBind();
                GetNewsItems(url);
            }
            base.OnInit(e);
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