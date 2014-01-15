using System.Web.Mvc;

namespace N2.Web.Mvc.Html
{
    public static class ContentHtmlExtensions
    {
        // content helper

        public static ViewContentHelper Content(this HtmlHelper html)
        {
            string key = "ContentHelperOf" + html.GetHashCode();
            var content = html.ViewContext.ViewData[key] as ViewContentHelper;
            if (content == null)
                html.ViewContext.ViewData[key] = content = new ViewContentHelper(html);
            return content;
        }

        public static bool HasValue(this HtmlHelper html, string detailName)
        {
            var item = html.CurrentItem();
            return item != null && item[detailName] != null;
        }

        public static bool ValueEquals<TData>(this HtmlHelper html, string detailName, TData expectedValue)
        {
            var item = html.CurrentItem();
            if (item == null)
                return false;

            object value = item[detailName];
            if (value == null && expectedValue == null)
                return true;
            if (value == null || expectedValue == null)
                return false;
            if (!(value is TData))
                return false;
            return expectedValue.Equals((TData)value);          
        }
    }
}
