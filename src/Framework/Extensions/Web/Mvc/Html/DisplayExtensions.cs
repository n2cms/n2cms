using System;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace N2.Web.Mvc.Html
{
    public static class DisplayExtensions
    {
        static DisplayExtensions()
        {
            DisplayableFactory = (html, propertyName, item) => new Displayable(html, propertyName, item);
        }

        public static Func<HtmlHelper, string, ContentItem, Displayable> DisplayableFactory { get; set; }

        public static Displayable DisplayContent(this HtmlHelper helper, string detailName)
        {
            return helper.DisplayContent(helper.CurrentItem(), detailName);
        }

        public static Displayable DisplayContent(this HtmlHelper helper, ContentItem item, string detailName)
        {
            return DisplayableFactory(helper, detailName, item);
        }

        public static Displayable DisplayContent<TItem>(this HtmlHelper<TItem> helper,
                                                 Expression<Func<TItem, object>> expression)
            where TItem : ContentItem
        {
            var member = (MemberExpression) expression.Body;
            return helper.DisplayContent(member.Member.Name);
        }

        public static Displayable DisplayContent<TItem, TProp>(this HtmlHelper<TItem> helper,
                                                 Expression<Func<TItem, TProp>> expression)
            where TItem : ContentItem
        {
            var member = (MemberExpression)expression.Body;
            return helper.DisplayContent(member.Member.Name);
        }

        public static Displayable DisplayContent<TItem>(this HtmlHelper helper, TItem item, Expression<Func<TItem, object>> expression)
            where TItem : ContentItem
        {
            var member = (MemberExpression) expression.Body;
            return helper.DisplayContent(item, member.Member.Name);
        }

        public static Displayable DisplayContent<TItem, TProp>(this HtmlHelper helper, TItem item, Expression<Func<TItem, TProp>> expression)
            where TItem : ContentItem
        {
            var member = (MemberExpression)expression.Body;
            return helper.DisplayContent(item, member.Member.Name);
        }

        public static void RenderDisplay(this HtmlHelper helper, string detailName)
        {
            helper.RenderDisplay(helper.CurrentItem(), detailName);
        }

        public static void RenderDisplay(this HtmlHelper helper, ContentItem item, string detailName)
        {
            DisplayableFactory(helper, detailName, item)
                .Render(helper.ViewContext.Writer);
        }



        public static void RenderDisplay<TItem>(this HtmlHelper<TItem> helper,
                                                 Expression<Func<TItem, object>> expression)
            where TItem : ContentItem
        {
            var member = (MemberExpression)expression.Body;
            helper.RenderDisplay(member.Member.Name);
        }

        public static void RenderDisplay<TItem, TProp>(this HtmlHelper<TItem> helper,
                                                 Expression<Func<TItem, TProp>> expression)
            where TItem : ContentItem
        {
            var member = (MemberExpression)expression.Body;
            helper.RenderDisplay(member.Member.Name);
        }

        public static void RenderDisplay<TItem>(this HtmlHelper helper, TItem item, Expression<Func<TItem, object>> expression)
            where TItem : ContentItem
        {
            var member = (MemberExpression)expression.Body;
            helper.DisplayContent(item, member.Member.Name).Render(helper.ViewContext.Writer);
        }

        public static void RenderDisplay<TItem, TProp>(this HtmlHelper helper, TItem item, Expression<Func<TItem, TProp>> expression)
            where TItem : ContentItem
        {
            var member = (MemberExpression)expression.Body;
            helper.DisplayContent(item, member.Member.Name).Render(helper.ViewContext.Writer);
        }
    }
}
