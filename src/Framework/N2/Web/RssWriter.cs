using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using N2.Definitions;
using N2.Engine;
using N2.Persistence.Serialization;
using N2.Persistence;

namespace N2.Web
{
    /// <summary>
    /// Writes an RSS feed to an output stream.
    /// </summary>
    [Service]
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
            if(feed.Published != null)
                xtw.WriteElementString("pubDate", GetDate(feed.Published.Value));
            xtw.WriteElementString("lastBuildDate", GetDate(N2.Utility.CurrentTime()));
            xtw.WriteElementString("generator", "N2 CMS");
            xtw.WriteElementString("managingEditor", feed.Author);
        }

        private string GetLink(string url)
        {
            return context.Url.HostUrl + url;
        }

        private string GetDate(DateTime dateTime)
        {
            // The datetime should in format like: Sat, 07 Sep 2002 00:00:01 GMT
            // http://asg.web.cmu.edu/rfc/rfc822.html#sec-5
            return TimeZone.CurrentTimeZone.ToUniversalTime(dateTime).ToString("R");
        }

        private void WriteItem(ISyndicatable item, XmlTextWriter xtw)
        {
            using (new ElementWriter("item", xtw))
            {
                xtw.WriteElementString("title", item.Title);
                xtw.WriteElementString("link", GetLink(item.Url));
                xtw.WriteElementString("description", item.Summary);
                if (item.Published != null)
                    xtw.WriteElementString("pubDate", GetDate(item.Published.Value));
                //xtw.WriteElementString("guid", "urn:uuid:" + item.MessageID);
            }
        }
    }
}
