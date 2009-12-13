using System;
using System.Linq.Expressions;
using N2.Web.UI;
using System.Web.Mvc;

namespace N2.Web.Mvc.Html
{
	public static class EditableDisplayableExtensions
	{

        public static EditableDisplayable EditableDisplay<TItem>(this HtmlHelper helper, string detailName)
            where TItem : ContentItem
        {
            return new EditableDisplayable(helper.ViewContext, Context.Current.Resolve<ITemplateRenderer>(), detailName);
        }

        public static EditableDisplayable EditableDisplay<TItem>(this HtmlHelper helper, Expression<Func<TItem, object>> expression)
            where TItem : ContentItem
        {
            var member = (MemberExpression)expression.Body;

            return helper.EditableDisplay<TItem>(member.Member.Name);
        }

        public static EditableDisplayable EditableDisplay<TItem>(this HtmlHelper helper, TItem item, string detailName)
            where TItem : ContentItem
        {
            return new EditableDisplayable(helper.ViewContext, Context.Current.Resolve<ITemplateRenderer>(), detailName, item);
        }

        public static EditableDisplayable EditableDisplay<TItem>(this HtmlHelper helper, TItem item, Expression<Func<TItem, object>> expression)
            where TItem : ContentItem
        {
            var member = (MemberExpression)expression.Body;

            return helper.EditableDisplay<TItem>(item, member.Member.Name);
        }




        ///////////////////////////////////////////////////////////////////////////////////////////




		public static EditableDisplayable EditableDisplay<TItem>(this IItemContainer<TItem> container, string detailName)
			where TItem : ContentItem
		{
			return new EditableDisplayable(container.ViewContext(), Context.Current.Resolve<ITemplateRenderer>(), detailName);
		}

		public static EditableDisplayable EditableDisplay<TItem>(this IItemContainer<TItem> container, Expression<Func<TItem, object>> expression)
			where TItem : ContentItem
		{
			var member = (MemberExpression)expression.Body;

			return container.EditableDisplay(member.Member.Name);
		}

		public static EditableDisplayable EditableDisplay<TItem>(this IItemContainer container, TItem item, string detailName)
			where TItem : ContentItem
		{
			return new EditableDisplayable(container.ViewContext(), Context.Current.Resolve<ITemplateRenderer>(), detailName, item);
		}

		public static EditableDisplayable EditableDisplay<TItem>(this IItemContainer container, TItem item, Expression<Func<TItem, object>> expression)
			where TItem : ContentItem
		{
			var member = (MemberExpression)expression.Body;

			return container.EditableDisplay(item, member.Member.Name);
		}

	}

	public class EditableDisplayable : Displayable
	{
        public EditableDisplayable(ViewContext viewContext, ITemplateRenderer templateRenderer, string detailName)
			: base(viewContext, templateRenderer, detailName)
		{
		}
        public EditableDisplayable(ViewContext viewContext, ITemplateRenderer templateRenderer, string detailName, ContentItem item)
			: base(viewContext, templateRenderer, detailName, item)
		{
		}
	}
}