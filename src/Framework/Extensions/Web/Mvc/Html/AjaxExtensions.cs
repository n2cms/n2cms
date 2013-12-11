using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace N2.Web.Mvc.Html
{
    public static class AjaxExtensions
    {
        public static IDisposable BeginAsyncAction(this HtmlHelper helper, string actionName)
        {
            return helper.BeginAsyncAction(actionName, new RouteValueDictionary());
        }

        public static IDisposable BeginAsyncAction(this HtmlHelper helper, string actionName, object routeValues)
        {
            return helper.BeginAsyncAction(actionName, new RouteValueDictionary(routeValues));
        }

        public static IDisposable BeginAsyncAction(this HtmlHelper helper, string actionName, RouteValueDictionary routeValues)
        {
            var id = "async" + Guid.NewGuid().ToString().Replace('-', '_');
            var url = new UrlHelper(helper.ViewContext.RequestContext).Action(actionName, routeValues);

            var tag = new System.Web.Mvc.TagBuilder("div");
            tag.Attributes["id"] = id;
            tag.AddCssClass("async loading");

            helper.ViewContext.Writer.Write(tag.ToString(TagRenderMode.StartTag));

            var asyncLoadScript = string.Format(@"<script type='text/javascript'>//<![CDATA[
jQuery(document).ready(function(){{jQuery('#{0}').load('{1}');}});//]]></script>", id, url);

            var end = tag.ToString(TagRenderMode.EndTag) + asyncLoadScript;
            
            return new WritesOnDispose(helper, end);
        }

        class WritesOnDispose : IDisposable
        {
            HtmlHelper helper;
            string endHtml;

            public WritesOnDispose(HtmlHelper helper, string htmlToWrite)
            {
                this.helper = helper;
                this.endHtml = htmlToWrite;
            }

            #region IDisposable Members

            public void Dispose()
            {
                helper.ViewContext.Writer.Write(endHtml);
            }

            #endregion
        }
    }
}
