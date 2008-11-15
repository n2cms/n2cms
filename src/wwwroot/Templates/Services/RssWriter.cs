using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using N2.Serialization;
using N2.Web;

namespace N2.Templates.Services
{
    /// <summary>
    /// Writes an RSS feed to an output stream.
    /// </summary>
    public class RssWriter
    {
        private readonly IWebContext context;

        public RssWriter(IWebContext context)
        {
            this.context = context;
        }

        public virtual void Write(TextWriter output, IFeed feed)
        {
            XmlTextWriter xtw = new XmlTextWriter(output);
            xtw.Formatting = Formatting.Indented;
            xtw.WriteStartDocument();

            using (ElementWriter rssElement = new ElementWriter("rss", xtw))
            {
                rssElement.WriteAttribute("version", "2.0");
                rssElement.WriteAttribute("xmlns", "xsd", null, "http://www.w3.org/2001/XMLSchema");
                using (new ElementWriter("channel", xtw))
                {
                    IEnumerable<ISyndicatable> items = feed.GetItems();
                    WriteChannelInfo(xtw, feed);
                    foreach (ISyndicatable item in items)
                    {
                        WriteItem(item, xtw);
                    }
                }
            }
            xtw.WriteEndDocument();
        }


        private void WriteChannelInfo(XmlTextWriter xtw, IFeed feed)
        {
            xtw.WriteElementString("title", feed.Title);
            xtw.WriteElementString("link", GetLink(feed.Url));
            xtw.WriteElementString("description", feed.Tagline);
            xtw.WriteElementString("language", "en-us");
            xtw.WriteElementString("pubDate", feed.Published.ToString());
            xtw.WriteElementString("lastBuildDate", DateTime.Now.ToString());
            xtw.WriteElementString("generator", "N2 CMS");
            xtw.WriteElementString("managingEditor", feed.Author);
        }

        private string GetLink(string url)
        {
            return context.Url.HostUrl + url;
        }

        private void WriteItem(ISyndicatable item, XmlTextWriter xtw)
        {
            using (new ElementWriter("item", xtw))
            {
                xtw.WriteElementString("title", item.Title);
                xtw.WriteElementString("link", GetLink(item.Url));
                xtw.WriteElementString("description", item.Summary);
                xtw.WriteElementString("pubDate", item.Published.ToString());
                //xtw.WriteElementString("guid", "urn:uuid:" + item.MessageID);
            }
        }
    }
}