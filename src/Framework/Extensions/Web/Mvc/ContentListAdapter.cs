using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using N2.Engine;
using N2.Web.UI;

namespace N2.Web.Mvc
{
	[Controls(typeof(ContentList))]
	public class ContentListController
	{
	}

	[Adapts(typeof(ContentList))]
	public class ContentListAdapter : MvcAdapter
	{
		public static string GetHtml(N2.ContentItem model)
		{
			if (model == null)
				return ("{model = null}"); // nothing to do 
			if (!(model is ContentList))
				return ("{adapter failure - Model is not a NewsList}"); // nothing to do 

			var currentItem = model as ContentList;
			var allNews = new List<ContentItem>();
			var containerLinks = currentItem.Containers as IEnumerable<ContentListContainerLink>;
			foreach (var containerLink in containerLinks.Where(c => c.Container.IsPage))
			{
				var container = containerLink.Container;
				foreach (var newsItem in container.GetChildren(new N2.Collections.TypeFilter(typeof(ContentPage))))
					if (newsItem.Published.HasValue) //TODO: Show unpublished news (e.g. calendar items)
						allNews.Add(newsItem);
			}

			IEnumerable<ContentItem> newsEnumerable = allNews.Where(a => a.IsPage && a.Published.HasValue);
			{
				// apply sort order ***
				if (currentItem.SortByDate == SortMode.Ascending)
					newsEnumerable = allNews.OrderBy(a => a.Published != null ? a.Published.Value : new DateTime());
				else
					newsEnumerable = allNews.OrderByDescending(a => a.Published != null ? a.Published.Value : new DateTime());

				// apply filter ***

				if (!currentItem.ShowFutureEvents)
					newsEnumerable = newsEnumerable.Where(a => a.Published != null && a.Published.Value <= DateTime.Now);

				if (!currentItem.ShowPastEvents)
					newsEnumerable = newsEnumerable.Where(a => a.Published != null && a.Published.Value >= DateTime.Now);

				if (currentItem.MaxNews > 0)
					newsEnumerable = newsEnumerable.Take(currentItem.MaxNews);
			}

			var sb = new System.Text.StringBuilder(50 * 1024);
			if (!String.IsNullOrEmpty(currentItem.Title))
			{
				sb.AppendFormat("<h{0}>{1}</h{0}>", (int)currentItem.TitleLevel, currentItem.Title);
			}

			DateTime? lastDate = null;
			var regex = new System.Text.RegularExpressions.Regex("\\$\\$(.*?)\\$\\$");
			// why no ForEach here? it turns out that the C# compiler changed the behavior RE: how it deals with foreach
			// variables accessed from within lambda expressions. 
			using (var itemEnumerator = newsEnumerable.GetEnumerator())
				while (itemEnumerator.MoveNext())
				{
					ContentItem item = itemEnumerator.Current;
					var itemPublished = item.Published == null ? new DateTime() : item.Published.Value;
					if (currentItem.GroupByMonth && (lastDate == null || lastDate.Value.Month != itemPublished.Month))
					{
						// new month ***
						sb.AppendFormat("<h2>{0:MMMM yyyy}</h2>\n", itemPublished);
						lastDate = item.Published.Value;
					}

					var showTitle = !String.IsNullOrEmpty(item.Title);// && item.ShowTitle
					var text = (item.GetDetail("Text") ?? "").ToString();
					var summary = (item.GetDetail("Summary") ?? "").ToString();

					// display either full article or abstract + link ***
					switch (currentItem.DisplayMode)
					{
						case NewsDisplayMode.TitleAndText:
							if (showTitle)
								sb.AppendFormat("<h{1}>{0}</h{1}>\n", item.Title ?? "Untitled", (int)currentItem.TitleLevel + 1);
							Debug.Assert(item.Published != null, "item.Published != null");
							sb.AppendFormat("<div class=\"date\">{0:MMMM d, yyyy}</div>\n", item.Published.Value);
							sb.AppendFormat("<div class=\"article\">\n{0}\n</div>\n", text);
							break;

						case NewsDisplayMode.TitleAndAbstract:
							if (!String.IsNullOrEmpty(text))
							{
								if (showTitle)
									sb.AppendFormat("<h{2}><a href=\"{1}\">{0}</a></h{2}>\n", item.Title ?? "Untitled", item.Url, (int)currentItem.TitleLevel + 1);
								Debug.Assert(item.Published != null, "item.Published != null");
								sb.AppendFormat("<div class=\"date\">{0:MMMM d, yyyy}</div>\n", item.Published.Value);
								sb.AppendFormat("<div class=\"abstract\">\n{0}\n</div>\n", summary);
								sb.AppendFormat("<a href=\"{0}\">Read more...</a>\n", item.Url);
							}
							else
							{
								if (showTitle)
									sb.AppendFormat("<h{1}>{0}</h{1}>\n", item.Title ?? "Untitled", (int)currentItem.TitleLevel + 1);
								Debug.Assert(item.Published != null, "item.Published != null");
								sb.AppendFormat("<div class=\"date\">{0:MMMM d, yyyy}</div>\n", item.Published.Value);
								sb.AppendFormat("<div class=\"abstract\">\n{0}\n</div>\n", summary);
							}
							break;

						case NewsDisplayMode.TitleLinkOnly:
							sb.AppendFormat("<h{2}><a href=\"{1}\">{0}</a></h{2}>\n", item.Title ?? "Untitled", item.Url, (int)currentItem.TitleLevel + 1);
							Debug.Assert(item.Published != null, "item.Published != null");
							sb.AppendFormat("<div class=\"date\">{0:MMMM d, yyyy}</div>\n", item.Published.Value);
							break;

						case NewsDisplayMode.HtmlItemTemplate:
							if (String.IsNullOrWhiteSpace(currentItem.HtmlItemTemplate))
							{
								break;
							}
							if (!currentItem.HtmlItemTemplate.Contains('$'))
							{
								sb.Append(currentItem.HtmlItemTemplate);
								break;
							}
							sb.Append(
								/* matches $$PropertyName:FormatString$$ and $$PropertyName$$ */
								regex.Replace(currentItem.HtmlItemTemplate, match => /* sorry this is so cryptic. Love, Ben :-) */
									                                            {
										                                            var pn = match.Groups[1].Value.Split(new[] {':'}, 2);
										                                            try
										                                            {
											                                            var d1 = item.GetDetail(pn[0]);
											                                            if (d1 == null)
												                                            return String.Concat('{', pn[0], ":null}");
											                                            return (pn.Length == 2
												                                                    ? String.Format(String.Concat("{0:", pn[1], '}'), d1)
												                                                    : d1.ToString());
										                                            }
										                                            catch (Exception err)
										                                            {
											                                            return err.ToString();
										                                            }
									                                            }));
							break;

						default:
							Debug.Assert(false, "Assertion failed due to invalid NewsDisplayMode.");
							break;
					}
				}

			return sb.ToString();
		}

		public override void RenderTemplate(System.Web.Mvc.HtmlHelper html, ContentItem model)
		{
			html.ViewContext.Writer.Write(GetHtml(model));
		}
	}
}