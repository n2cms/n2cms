using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.WebPages;
using System.Web.Mvc;

namespace N2.Web.Mvc.Html
{
	/// <remarks>This code is here since it has dependencies on ASP.NET 3.0 which isn't a requirement for N2 in general.</remarks>
	public static class HtmlExtensions
	{
		public static IHtmlString ToHtmlString(this object instance)
		{
			if (instance == null)
				return null;
			return new HtmlString(instance.ToString());
		}

		/// <summary>Loops an enumeration of items and applies html templates.</summary>
		/// <typeparam name="T">The type of item to enumerate.</typeparam>
		/// <param name="html">Placeholder.</param>
		/// <param name="items">The items to loop.</param>
		/// <param name="template">The item template, repeated for each item.</param>
		/// <param name="wrapper">A wrapper around all items, applied if there are any items.</param>
		/// <param name="separator">A separator between items.</param>
		/// <param name="empty">Template applied when no items.</param>
		/// <returns>A helper result used to render to the output stream or create a string.</returns>
		public static HelperResult Loop<T>(this HtmlHelper html,
			IEnumerable<T> items,
			Func<ListTemplate<T>, HelperResult> template,
			Func<Template<IEnumerable<T>>, HelperResult> wrapper = null,
			Func<ListTemplate<T>, HelperResult> separator = null,
			Func<Template<IEnumerable<T>>, HelperResult> empty = null)
		{
			return new HelperResult((tw) =>
			{
				using (var enumerator = items.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						Action<TextWriter> renderContents = (tw2) =>
						{
							var ctx = new ListTemplate<T> { First = true };
							while (true)
							{
								ctx.Data = enumerator.Current;
								ctx.Last = enumerator.MoveNext() == false;

								template(ctx).WriteTo(tw2);

								if (ctx.Last)
									break;
								else if (separator != null)
									separator(ctx).WriteTo(tw2);

								ctx.Index++;
								if (ctx.First)
									ctx.First = false;
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
					else if (empty != null)
					{
						empty(new Template<IEnumerable<T>> { Data = items }).WriteTo(tw);
					}
				}
			});
		}

		/// <summary>Loops an enumeration of items creating an unordered list.</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="html"></param>
		/// <param name="items"></param>
		/// <param name="template"></param>
		/// <param name="empty"></param>
		/// <returns></returns>
		public static HelperResult UnorderedList<T>(this HtmlHelper html, IEnumerable<T> items,
			Func<ListTemplate<T>, HelperResult> template,
			Func<Template<IEnumerable<T>>, HelperResult> empty = null)
		{
			return Loop(html, items,
				template: (lt) => new HelperResult((tw) => { tw.Write("<li>"); template(lt).WriteTo(tw); tw.Write("</li>"); }),
				wrapper: (lt) => new HelperResult((tw) => { tw.Write("<ul>"); lt.RenderContents().WriteTo(tw); tw.Write("</ul>"); }),
				empty: empty);
		}
	}
}