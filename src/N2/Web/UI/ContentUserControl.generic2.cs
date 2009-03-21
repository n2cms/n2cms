using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Web.UI
{
	/// <summary>A user control that can be dynamically created, bound to non-page items and added to a page.</summary>
	/// <typeparam name="TPage">The type of page item this user control will have to deal with.</typeparam>
    /// <typeparam name="TItem">The type of non-page (data) item this user control will be bound to.</typeparam>
	public abstract class ContentUserControl<TPage, TItem> : ContentUserControl<TPage>, IItemContainer, IContentTemplate
		where TPage : N2.ContentItem
		where TItem : N2.ContentItem
	{
		private TItem currentItem = null;

		/// <summary>Gets the current data item of the dynamically added part.</summary>
		public new TItem CurrentItem
		{
			get { return this.currentItem ?? (currentItem = ItemUtility.CurrentContentItem as TItem); }
			set { currentItem = value; }
		}

		/// <summary>Gets whether the current data item is referenced in the query string. This usually occurs when the item is selected in edit mode.</summary>
		public bool IsHighlighted
		{
			get
			{
				if (!string.IsNullOrEmpty(Request.QueryString[PathData.ItemQueryKey]))
					return this.CurrentItem.ID == int.Parse(Request.QueryString[PathData.ItemQueryKey]);
				return false;
			}
		}

		#region IItemContainer & IContentTemplate
		ContentItem IContentTemplate.CurrentItem
		{
			get { return CurrentItem; }
			set { CurrentItem = ItemUtility.EnsureType<TItem>(value); }
		}

		ContentItem IItemContainer.CurrentItem
		{
			get { return CurrentItem; }
		}
		#endregion
	}
}
