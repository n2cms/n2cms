using System;
using System.Linq.Expressions;
using N2.Web.UI;
using System.Web.Mvc;

namespace N2.Web.Mvc.Html
{
    internal static class IItemContainerExtensions
    {
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

	public static class DisplayExtensions
	{
        public static Displayable Display<TItem>(this HtmlHelper helper, string detailName)
			where TItem : ContentItem
		{
            return new Displayable(helper.ViewContext, Context.Current.Resolve<ITemplateRenderer>(), detailName);
		}

		public static Displayable Display<TItem>(this HtmlHelper<TItem> helper,
		                                         Expression<Func<TItem, object>> expression)
			where TItem : ContentItem
		{
            var member = (MemberExpression) expression.Body;

            return helper.Display<TItem>(member.Member.Name);
		}

        public static Displayable Display<TItem>(this HtmlHelper helper, TItem item, string detailName)
			where TItem : ContentItem
		{
            return new Displayable(helper.ViewContext, Context.Current.Resolve<ITemplateRenderer>(), detailName);
		}

        public static Displayable Display<TItem>(this HtmlHelper helper, TItem item, Expression<Func<TItem, object>> expression)
			where TItem : ContentItem
		{
            var member = (MemberExpression) expression.Body;

            return helper.Display(item, member.Member.Name);
		}





        [Obsolete("Prefer Html.Display")]
        public static Displayable Display<TItem>(this IItemContainer<TItem> container, string detailName)
            where TItem : ContentItem
        {

            return new Displayable(container.ViewContext(), Context.Current.Resolve<ITemplateRenderer>(), detailName);
        }

        [Obsolete("Prefer Html.Display")]
        public static Displayable Display<TItem>(this IItemContainer<TItem> container,
                                                 Expression<Func<TItem, object>> expression)
            where TItem : ContentItem
        {
            var member = (MemberExpression)expression.Body;

            return container.Display(member.Member.Name);
        }

        [Obsolete("Prefer Html.Display")]
        public static Displayable Display<TItem>(this IItemContainer container, TItem item, string detailName)
            where TItem : ContentItem
        {
            return new Displayable(container.ViewContext(), Context.Current.Resolve<ITemplateRenderer>(), detailName);
        }

        [Obsolete("Prefer Html.Display")]
        public static Displayable Display<TItem>(this IItemContainer container, TItem item, Expression<Func<TItem, object>> expression)
            where TItem : ContentItem
        {
            var member = (MemberExpression)expression.Body;

            return container.Display(item, member.Member.Name);
        }
	}
}