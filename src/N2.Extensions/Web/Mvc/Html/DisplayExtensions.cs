using System;
using System.Linq.Expressions;
using N2.Web.UI;
using System.Web.Mvc;

namespace N2.Web.Mvc.Html
{
	public static class DisplayExtensions
	{
        public static Displayable DisplayContent(this HtmlHelper helper, string detailName)
		{
			return helper.DisplayContent(helper.CurrentItem(), detailName);
		}

		public static Displayable DisplayContent(this HtmlHelper helper, ContentItem item, string detailName)
		{
            return new Displayable(helper.ViewContext, Context.Current.Resolve<ITemplateRenderer>(), detailName, item);
		}



		public static Displayable DisplayContent<TItem>(this HtmlHelper<TItem> helper,
		                                         Expression<Func<TItem, object>> expression)
			where TItem : ContentItem
		{
            var member = (MemberExpression) expression.Body;
			return helper.DisplayContent(member.Member.Name);
		}

		public static Displayable DisplayContent<TItem>(this HtmlHelper helper, TItem item, Expression<Func<TItem, object>> expression)
			where TItem : ContentItem
		{
            var member = (MemberExpression) expression.Body;
			return helper.DisplayContent(item, member.Member.Name);
        }



        public static void RenderDisplay(this HtmlHelper helper, string detailName)
        {
            helper.RenderDisplay(helper.CurrentItem(), detailName);
        }

        public static void RenderDisplay(this HtmlHelper helper, ContentItem item, string detailName)
        {
            new Displayable(helper.ViewContext, Context.Current.Resolve<ITemplateRenderer>(), detailName, item)
                .Render(helper.ViewContext.HttpContext.Response.Output);
        }



        public static void RenderDisplay<TItem>(this HtmlHelper<TItem> helper,
                                                 Expression<Func<TItem, object>> expression)
            where TItem : ContentItem
        {
            var member = (MemberExpression)expression.Body;
            helper.RenderDisplay(member.Member.Name);
        }

        public static void RenderDisplay<TItem>(this HtmlHelper helper, TItem item, Expression<Func<TItem, object>> expression)
            where TItem : ContentItem
        {
            var member = (MemberExpression)expression.Body;
			helper.DisplayContent(item, member.Member.Name).Render(helper.ViewContext.HttpContext.Response.Output);
        }



        [Obsolete("Use Html.Display")]
		public static Displayable DisplayContent<TItem>(this IItemContainer<TItem> container, string detailName)
            where TItem : ContentItem
        {

            return new Displayable(container.ViewContext(), Context.Current.Resolve<ITemplateRenderer>(), detailName, container.CurrentItem);
        }

        [Obsolete("Use Html.Display")]
		public static Displayable DisplayContent<TItem>(this IItemContainer<TItem> container,
                                                 Expression<Func<TItem, object>> expression)
            where TItem : ContentItem
        {
            var member = (MemberExpression)expression.Body;

			return container.DisplayContent(member.Member.Name);
        }

        [Obsolete("Use Html.Display")]
		public static Displayable DisplayContent<TItem>(this IItemContainer container, TItem item, string detailName)
            where TItem : ContentItem
        {
            return new Displayable(container.ViewContext(), Context.Current.Resolve<ITemplateRenderer>(), detailName, item);
        }

        [Obsolete("Use Html.Display")]
        public static Displayable Display<TItem>(this IItemContainer container, TItem item, Expression<Func<TItem, object>> expression)
            where TItem : ContentItem
        {
            var member = (MemberExpression)expression.Body;

			return container.DisplayContent(item, member.Member.Name);
        }



        internal static ViewContext ViewContext(this IItemContainer container)
        {
            if (container is ViewPage)
                return (container as ViewPage).ViewContext;
            if (container is ViewUserControl)
                return (container as ViewUserControl).ViewContext;
            if (container is ViewMasterPage)
                return (container as ViewMasterPage).ViewContext;

            throw new NotImplementedException("Please use Html.Display instead of this.Display (" + container.GetType() + ")");
        }
	}
}