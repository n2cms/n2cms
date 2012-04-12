using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Xml;
using N2.Engine.Globalization;
using N2.Engine;
using System.Security.Principal;

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

		public IEngine Engine { get { return Context.Current; } }

		public string GetSiteMap(HttpContext context)
		{
			ContentItem rootItem = Engine.RequestContext.CurrentPath.StopItem;
			ILanguageGateway lg = Engine.Resolve<ILanguageGateway>();
			string cacheKey = "Googlesitemap:" + rootItem.ToString();

			// As this is a heavy operation, use the cache
			if (context.Cache[cacheKey] != null)
				return context.Cache[cacheKey].ToString();

			string baseUrl = GetBaseUrl(context);
			IList<ContentItem> list = new List<ContentItem>();
			RecurseTree(list, rootItem, lg.GetLanguage(rootItem).LanguageCode);

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

				// <urlset>
				writer.WriteEndElement();
			}

			stringWriter.Flush();

			// Add to the cache for 30 minutes.
			context.Cache.Add(cacheKey, builder.ToString(), null, DateTime.Today.AddMinutes(30), Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);

			return builder.ToString();
		}

		/// <summary>
		/// Get base URL to publish for items in the sitemap.
		/// </summary>
		public virtual string GetBaseUrl(HttpContext context)
		{
			return "http://" + context.Request.Url.Authority;
		}

		/// <summary>
		/// Builds up the entire site tree recursively, adding items to the list
		/// </summary>
		/// <param name="list">This should be an empty list</param>
		/// <param name="parent">This should be called using the root item</param>
		private void RecurseTree(IList<ContentItem> list, ContentItem parent, string language)
		{
			foreach (var item in parent.Children.FindNavigatablePages())
			{
				if (!item.IsAuthorized(new GenericPrincipal(new GenericIdentity(""), null))) continue;
				if (item is ILanguage && ((ILanguage)item).LanguageCode != language) continue;
				if (item.Visible && item.IsPage) list.Add(item);
				if (item.IsPage) RecurseTree(list, item, language);
			}
		}
	}
}
