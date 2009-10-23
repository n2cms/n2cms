using System;
using System.Linq.Expressions;
using N2.Web.UI;

namespace N2.Web.Mvc.Html
{
	public static class DisplayExtensions
	{
		public static Displayable Display<TItem>(this IItemContainer<TItem> container, string detailName)
			where TItem : ContentItem
		{
			return new Displayable(Context.Current.Resolve<ITemplateRenderer>(), container, detailName);
		}

		public static Displayable Display<TItem>(this IItemContainer<TItem> container,
		                                         Expression<Func<TItem, object>> expression)
			where TItem : ContentItem
		{
			var member = (MemberExpression) expression.Body;

			return container.Display(member.Member.Name);
		}

		public static Displayable Display<TItem>(this IItemContainer container, TItem item, string detailName)
			where TItem : ContentItem
		{
			return new Displayable(Context.Current.Resolve<ITemplateRenderer>(), container, detailName, item);
		}

		public static Displayable Display<TItem>(this IItemContainer container, TItem item, Expression<Func<TItem, object>> expression)
			where TItem : ContentItem
		{
			var member = (MemberExpression) expression.Body;

			return container.Display(item, member.Member.Name);
		}
	}
}