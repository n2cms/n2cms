using System;
using N2.Web.UI;

namespace N2.Web.Mvc.Html
{
	/// <summary>
	/// Helper class for loosley-typed evaluation of Detail properties on the current item
	/// </summary>
	public static class DetailExtensions
	{
		public static string Detail<TItem>(this IItemContainer container, string detailName)
			where TItem : ContentItem
		{
			return Convert.ToString(container.CurrentItem[detailName]);
		}
	}
}