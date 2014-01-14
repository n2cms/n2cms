using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.WebPages;
using System.Web.Mvc;
using N2.Collections;
using System.Web.Routing;
using N2.Web.UI;

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
            return Loop<T>(items, template, wrapper, separator, empty);
        }

        private static HelperResult Loop<T>(
            IEnumerable<T> items, 
            Func<ListTemplate<T>, HelperResult> template, 
            Func<Template<IEnumerable<T>>, HelperResult> wrapper, 
            Func<ListTemplate<T>, HelperResult> separator, 
            Func<Template<IEnumerable<T>>, HelperResult> empty)
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
            Func<Template<IEnumerable<T>>, HelperResult> empty = null,
            object htmlAttributes = null)
        {
            return UnorderedList<T>(items, template, empty, htmlAttributes);
        }

        private static HelperResult UnorderedList<T>(IEnumerable<T> items, Func<ListTemplate<T>, HelperResult> template, Func<Template<IEnumerable<T>>, HelperResult> empty, object htmlAttributes)
        {
            return Loop(items,
                template: (lt) => new HelperResult((tw) => { tw.Write("<li>"); template(lt).WriteTo(tw); tw.Write("</li>"); }),
                wrapper: (lt) => new HelperResult((tw) => 
                {
                    tw.Write("<ul");
                    if (htmlAttributes != null)
                    {
                        foreach (var kvp in new RouteValueDictionary(htmlAttributes))
                        {
                            if (kvp.Value != null && !"".Equals(kvp.Value))
                            {
                                tw.Write(" " + kvp.Key + "=\"");
                                HttpUtility.HtmlAttributeEncode(kvp.Value.ToString(), tw);
                                tw.Write("\"");
                            }
                        }
                    }
                    tw.Write(">");
                    lt.RenderContents().WriteTo(tw);
                    tw.Write("</ul>");
                }),
                separator: null,
                empty: empty);
        }

        public static HelperResult OutputCache(this HtmlHelper html, 
            Func<string, HelperResult> template, 
            string cacheKey = null, 
            bool? perPage = null, 
            int? perAncestorAtLevel = null,
            DateTime? absoluteExpiration = null,
            TimeSpan? slidingExpiration = null)
        {
            if (html.ViewContext.RequestContext.HttpContext.Request.QueryString["edit"] != null)
                return template("");

            string composedKey = CreateCacheKey(html, template, cacheKey, perPage, perAncestorAtLevel);

            var cache = html.ContentEngine().Resolve<N2.Web.CacheWrapper>();

            var cachedResults = cache.Get<string>(composedKey);
            if (cachedResults != null)
            {
                return new HelperResult(tw => tw.Write(cachedResults));
            }

            return new HelperResult(tw =>
                {
                    using (var sw = new StringWriter())
                    {
                        var originalWriter = html.ViewContext.Writer;
                        try
                        {
                            html.ViewContext.Writer = sw; // in case something renders directly to the stream

                            template(composedKey).WriteTo(sw);
                            var results = sw.ToString();

                            cache.Add(composedKey, results, new CacheOptions { AbsoluteExpiration = absoluteExpiration ?? System.Web.Caching.Cache.NoAbsoluteExpiration, SlidingExpiration = slidingExpiration ?? System.Web.Caching.Cache.NoSlidingExpiration });

                            tw.Write(results);
                        }
                        finally
                        {
                            html.ViewContext.Writer = originalWriter;
                        }
                    }
                });
        }

        private static string CreateCacheKey(HtmlHelper html, Func<string, HelperResult> template, string cacheKey, bool? perPage, int? perAncestorAtLevel)
        {
            int perPageComponent = 0;
            if (perPage ?? !perAncestorAtLevel.HasValue)
            {
                var page = html.CurrentPage();
                if (page != null)
                    perPageComponent = page.ID;
            }
            int perAncestorComponent = 0;
            if (perAncestorAtLevel.HasValue)
            {
                var ancestor = html.Content().Traverse.AncestorAtLevel(perAncestorAtLevel.Value, fallbackToClosest: true);
                if (ancestor != null)
                    perAncestorComponent = ancestor.ID;
            }
            string composedKey = "Html.OutputCache"
                + cacheKey
                + template.Method.ToString()
                + perPageComponent
                + perAncestorComponent;
            return composedKey;
        }
    }
}
