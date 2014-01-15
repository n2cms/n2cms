using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using N2.Web.Parsing;
using N2.Web.Rendering;
using N2.Web.Wiki.Analyzers;
using System.Diagnostics;
using System.Web.Routing;
using System.Web;
using N2.Web.Mvc;
using N2.Web;
using System;
using N2.Web.Tokens;

namespace N2.Details
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DisplayableTokensAttribute : AbstractDisplayableAttribute, IContentTransformer
    {
        /// <summary>String that suffixes the detail name when tokens are stored in the detail collection.</summary>
        public const string CollectionSuffix = "_Tokens";

        private readonly Engine.Logger<DisplayableTokensAttribute> logger;

        public override Control AddTo(ContentItem item, string detailName, Control container)
        {
            using (var sw = new StringWriter())
            {
                var rc = new RenderingContext { Content = item, Displayable = this, Html = CreateHtmlHelper(item, sw), PropertyName = detailName };
                Render(rc, sw);

                var lc = new LiteralControl(sw.ToString());
                container.Controls.Add(lc);
                return lc;
            }
        }

        private static HtmlHelper CreateHtmlHelper(ContentItem item, TextWriter writer)
        {
            var httpContext = HttpContext.Current.GetHttpContextBase();
            var routeData = new RouteData();
            RouteExtensions.ApplyCurrentPath(routeData, "WebForms", "Index", new PathData(item.ClosestPage(), item));
            var cc = new ControllerContext() { HttpContext = httpContext, RequestContext = new RequestContext(httpContext, routeData), RouteData = routeData };
            return new HtmlHelper(
                new ViewContext(
                    cc,
                    new WebFormView(cc, HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath),
                    new ViewDataDictionary(),
                    new TempDataDictionary(),
                    writer),
                new ViewPage(),
                RouteTable.Routes);
        }



        #region IContentTransformer Members

        public ContentState ChangingTo
        {
            get { return ContentState.Published | ContentState.Draft; }
        }

        public bool Transform(ContentItem item)
        {
            string text = item[Name] as string;
            if (text != null)
            {
                string collectionName = Name + CollectionSuffix;
                int i = 0;
                var p = new TokenParser();
                foreach (var c in p.Parse(text).Where(c => c.Command != Parser.TextCommand))
                {
                    var dc = item.GetDetailCollection(collectionName, true);
                    var cd = ContentDetail.Multi(collectionName, stringValue: c.Tokens.Select(t => t.Fragment).StringJoin(), integerValue: c.Tokens.First().Index);
                    cd.EnclosingItem = item;
                    cd.EnclosingCollection = dc;

                    if (dc.Details.Count > i)
                        dc.Details[i] = cd;
                    else
                        dc.Details.Add(cd);
                    i++;
                }
                if (i > 0)
                {
                    var dc = item.GetDetailCollection(collectionName, true);
                    for (int j = dc.Details.Count - 1; j >= i; j--)
                    {
                        dc.Details.RemoveAt(j);
                    }
                    return true;
                }
                else if (i == 0)
                {
                    var dc = item.GetDetailCollection(collectionName, false);
                    if (dc != null)
                    {
                        item.DetailCollections.Remove(dc);
                        return true;
                    }
                }
            }
            return false;
        }

        #endregion

        internal void Render(RenderingContext context, TextWriter writer)
        {
            string text = context.Content[context.PropertyName] as string;
            if (text == null)
                return;

            var tokens = context.Content.GetDetailCollection(context.PropertyName + CollectionSuffix, false);
            if (tokens != null)
            {
                int lastFragmentEnd = 0;

                foreach (var detail in tokens.Details)
                {
                    var token = detail.ExtractToken();

                    if (lastFragmentEnd < token.Index)
                        writer.Write(text.Substring(lastFragmentEnd, token.Index - lastFragmentEnd));

                    ViewEngineResult vr = null;
                    if (context.Html.ViewContext.HttpContext.IsCustomErrorEnabled)
                    {
                        try
                        {
                            vr = ViewEngines.Engines.FindPartialView(context.Html.ViewContext, "TokenTemplates/" + token.Name);
                        }
                        catch (System.Exception ex)
                        {
                            logger.Error(ex);
                        }
                    }
                    else
                    {
                        vr = ViewEngines.Engines.FindPartialView(context.Html.ViewContext, "TokenTemplates/" + token.Name); // duplicated to preserve stack trace
                    }

                    if (vr != null && vr.View != null)
                    {
                        var viewData = new ViewDataDictionary(token.Value) { { "ParentViewContext", context.Html.ViewContext } };
                        viewData[RenderingExtensions.ContextKey] = context;
                        viewData[RenderingExtensions.TokenKey] = token;
                        var vc = new ViewContext(context.Html.ViewContext, vr.View, viewData, context.Html.ViewContext.TempData, writer);
                        vr.View.Render(vc, writer);
                    }
                    else
                        writer.Write(detail.StringValue);

                    lastFragmentEnd = token.Index + detail.StringValue.Length;
                }

                if (lastFragmentEnd < text.Length)
                {
                    writer.Write(text.Substring(lastFragmentEnd, text.Length - lastFragmentEnd));
                }
            }
            else
            {
                writer.Write(text);
            }
        }
    }
}
