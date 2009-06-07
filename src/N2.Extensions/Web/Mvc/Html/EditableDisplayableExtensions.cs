using System;
using System.Linq.Expressions;
using N2.Web.UI;

namespace N2.Web.Mvc.Html
{
	public static class EditableEditableDisplayableExtensions
	{
		public static EditableDisplayable EditableDisplay<TItem>(this IItemContainer<TItem> container, string detailName)
			where TItem : ContentItem
		{
			return new EditableDisplayable(Context.Current.Resolve<ITemplateRenderer>(), container, detailName);
		}

		public static EditableDisplayable EditableDisplay<TItem>(this IItemContainer<TItem> container, Expression<Func<TItem, object>> expression)
			where TItem : ContentItem
		{
			var member = (MemberExpression)expression.Body;

			return container.EditableDisplay(member.Member.Name);
		}

		public static EditableDisplayable EditableDisplay<TItem>(this IItemContainer<TItem> container, string detailName, TItem item)
			where TItem : ContentItem
		{
			return new EditableDisplayable(Context.Current.Resolve<ITemplateRenderer>(), container, detailName, item);
		}

		public static EditableDisplayable EditableDisplay<TItem>(this IItemContainer<TItem> container,
														Expression<Func<TItem, object>> expression, TItem item)
			where TItem : ContentItem
		{
			var member = (MemberExpression)expression.Body;

			return container.EditableDisplay(member.Member.Name, item);
		}

	}

	public class EditableDisplayable : Displayable
	{
		public EditableDisplayable(ITemplateRenderer templateRenderer, IItemContainer container, string detailName)
			: base(templateRenderer, container, detailName)
		{
		}

		public EditableDisplayable(ITemplateRenderer templateRenderer, IItemContainer container, string detailName, ContentItem item)
			: base(templateRenderer, container, detailName, item)
		{
		}
	}
}