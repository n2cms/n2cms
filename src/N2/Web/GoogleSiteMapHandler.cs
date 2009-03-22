using System;
using System.Collections.Generic;
using System.Web;
using System.Xml;
using System.Text;
using N2;
using System.IO;
using System.Web.Caching;

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
			context.Response.Write(GetSiteMap(context));
		}

		public string GetSiteMap(HttpContext context)
		{
			// As this is a heavy operation, use the cache
			if (context.Cache["Googlesitemap"] != null)
				return context.Cache["Googlesitemap"].ToString();

			string domain = "http://" + context.Request.Url.Authority;
			IList<ContentItem> list = new List<ContentItem>();
			ContentItem rootItem = N2.Find.RootItem;
			RecurseTree(list, rootItem);

			StringBuilder builder = new StringBuilder();
			StringWriter stringWriter = new StringWriter(builder);

			XmlWriterSettings settings = new XmlWriterSettings();
			settings.OmitXmlDeclaration = true;
			settings.Indent = true;
			settings.Encoding = Encoding.UTF8;

			using (XmlWriter writer = XmlWriter.Create(stringWriter, settings))
			{
				writer.WriteStartDocument();

				// <urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
				writer.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");

				foreach (var item in list)
				{
					// <url>
					writer.WriteStartElement("url");

					writer.WriteElementString("loc", domain + item.Url);
					writer.WriteElementString("lastmod", item.Published.GetValueOrDefault().ToString("yyyy-MM-dd")); // Google doesn't like IS0 8601/W3C 
					writer.WriteElementString("changefreq", "weekly"); // TODO make this a setting
					writer.WriteElementString("priority", "0"); // TODO make this a setting

					// </url>
					writer.WriteEndElement();
				}

				// <urlset>
				writer.WriteEndElement();
			}

			stringWriter.Flush();

			// Add to the cache for 3 days.
			context.Cache.Add("Googlesitemap", builder.ToString(), null, DateTime.Today.AddDays(3), Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);

			return builder.ToString();
		}

		/// <summary>
		/// Builds up the entire site tree recursively, adding items to the list
		/// </summary>
		/// <param name="list">This should be an empty list</param>
		/// <param name="parent">This should be called using the root item</param>
		private void RecurseTree(IList<ContentItem> list, ContentItem parent)
		{
			// TODO: add caching?
			foreach (var item in parent.GetChildren())
			{
				if (item.Visible && item.IsPage) // TODO make this a setting
				{
					list.Add(item);
					RecurseTree(list, item);
				}
			}
		}
	}
}
