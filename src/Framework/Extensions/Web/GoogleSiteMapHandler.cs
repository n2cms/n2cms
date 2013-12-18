using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Xml;
using N2.Engine.Globalization;
using N2.Engine;
using System.Security.Principal;
using N2.Definitions;
using N2.Web.UI;

namespace N2.Web
{
    /// <summary>
    /// Provides a sitemap format file (XML) for the site, accessed via
    /// http://www.yoursite.com/GoogleSiteMapHandler.axd
    /// 
    /// See sitemap.org for details on what's outputted.
    /// </summary>
    /// <remarks>
    /// This doesn't handle multiple sites and a lot of settings are hardcoded for now.
    /// 
    /// You can ping google to update your sitemap (along with the webmaster tools) using
    /// http://www.google.com/ping?sitemap=url
    /// </remarks>
    public class GoogleSiteMapHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Entry point for the handler.
        /// </summary>
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ClearHeaders();
            context.Response.ClearContent();

            context.Response.ContentType = "text/xml";

            context.Response.Write("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n");
            WriteSiteMap(context);

            SetOutputCache(context);
        }

        protected virtual void SetOutputCache(HttpContext context)
        {
            context.TrySetCompressionFilter();
            context.Response.SetOutputCache(Utility.CurrentTime().AddDays(1));
            context.Response.AddCacheDependency(new ContentCacheDependency(Engine.Persister));
        }

        public IEngine Engine { get { return Context.Current; } }

        public void WriteSiteMap(HttpContext context)
        {
            using (XmlWriter writer = CreateXmlWriter(context))
            {
                writer.WriteStartDocument();

                // <urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
                writer.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");

                string baseUrl = GetBaseUrl(context);
                var root = Engine.UrlParser.FindPath(context.Request.Url).StopItem;
                WriteItem(writer, baseUrl, root);
                var descendants = GetDescendants(root);
                foreach (var item in descendants)
                {
                    WriteItem(writer, baseUrl, item);
                }

                // <urlset>
                writer.WriteEndElement();
            }
        }

        protected virtual XmlWriter CreateXmlWriter(HttpContext context)
        {
            XmlWriterSettings settings = new XmlWriterSettings { OmitXmlDeclaration = true, Encoding = Encoding.UTF8 };
            return XmlWriter.Create(context.Response.Output, settings);
        }

        protected virtual void WriteItem(XmlWriter writer, string baseUrl, ContentItem item)
        {
            // <url>
            if (!item.Url.StartsWith("http"))
            {
                writer.WriteStartElement("url");

                writer.WriteElementString("loc", baseUrl + item.Url);
                writer.WriteElementString("lastmod", item.Published.GetValueOrDefault().ToString("yyyy-MM-dd")); // Google doesn't like IS0 8601/W3C 
                writer.WriteElementString("changefreq", "weekly"); // TODO make this a setting
                writer.WriteElementString("priority", "0"); // TODO make this a setting

                // </url>
                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Get base URL to publish for items in the sitemap.
        /// </summary>
        public virtual string GetBaseUrl(HttpContext context)
        {
            return "http://" + context.Request.Url.Authority;
        }

        public virtual IEnumerable<ContentItem> GetDescendants(ContentItem parent)
        {
            var children = parent.Children.FindPages();
            foreach (var child in children)
            {
                if (child is ILanguage) continue;
                if (child is IRedirect) continue;
                if (child is ISystemNode) continue;
                if (!child.Visible) continue;
                if (!child.IsAuthorized(new GenericPrincipal(new GenericIdentity(""), null))) continue;
                
                yield return child;

                foreach (var descendant in GetDescendants(child))
                {
                    yield return descendant;
                }
            }
        }
    }
}
