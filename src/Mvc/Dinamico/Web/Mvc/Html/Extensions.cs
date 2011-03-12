using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using N2.Definitions;
using N2.Details;
using System.Web.Hosting;
using System.Web.WebPages;
using System.IO;

namespace N2.Web.Mvc.Html
{
	public class ListTemplate<T> : Template<T>
	{
		public int Index { get; set; }
		public bool First { get; set; }
		public bool Last { get; set; }
	}

	public class Template<T>
	{
		public T Data { get; set; }
		public Action<TextWriter> ContentRenderer { get; set; }
		static Action<TextWriter> fallback = (tw) => { };

		public HelperResult RenderContents()
		{
			return new HelperResult(ContentRenderer ?? fallback);
		}
	}

	public static class Extensions
	{
		public static IHtmlString ToHtmlString(this object instance)
		{
			if (instance == null)
				return null;
			return new HtmlString(instance.ToString());
		}

		public static HelperResult Loop<T>(this HtmlHelper html, 
			IEnumerable<T> items, 
			Func<ListTemplate<T>, HelperResult> template,
			Func<Template<IEnumerable<T>>, HelperResult> wrapper = null,
			Func<Template<IEnumerable<T>>, HelperResult> empty = null)
		{
			return new System.Web.WebPages.HelperResult((tw) =>
				{
					using(var enumerator = items.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							var ctx = new ListTemplate<T> { First = true };
							Action<TextWriter> renderContents = (tw2) =>
								{
									while (true)
									{
										ctx.Data = enumerator.Current;
										ctx.Last = !enumerator.MoveNext();
										if(ctx.Data is ContentItem)
										{
											using(html.Content().BeginContentScope(ctx.Data as ContentItem))
											{
												template(ctx).WriteTo(tw2);
											}
										}
										else
										{
											template(ctx).WriteTo(tw2);
										}
										if(ctx.Last)
											break;
										ctx.Index++;
									}
								};
							if (wrapper != null)
							{
								wrapper(new Template<IEnumerable<T>> { Data = items, ContentRenderer = renderContents }).WriteTo(tw);
							}
							else
							{
								renderContents(tw);
							}
						}
						else if(empty != null)
						{
							empty(new Template<IEnumerable<T>> { Data = items }).WriteTo(tw);
						}
					}
				});
		}

		// content helper

		public static ContentHelper Content(this HtmlHelper html)
		{
			string key = "ContentHelperOf" + html.GetHashCode();
			var content = html.ViewContext.ViewData[key] as ContentHelper;
			if (content == null)
				html.ViewContext.ViewData[key] = content = new ContentHelper(html);
			return content;
		}
	}
}