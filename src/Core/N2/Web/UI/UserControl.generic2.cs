using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Web.UI
{
	/// <summary>A user control that can be dynamically created, bound to non-page items and added to a page.</summary>
	/// <typeparam name="TPage">The type of page item this user control will have to deal with.</typeparam>
	/// <typeparam name="TData">The type of non-page (data) item this user control will be bound to.</typeparam>
	public class UserControl<TPage, TItem> : UserControl<TPage>, IDataItemContainer
		where TPage : N2.ContentItem
		where TItem : N2.ContentItem
	{
		private TItem currentItem;

		/// <summary>Gets the current data item.</summary>
		[Obsolete("Use CurrentItem to access part specific data.")]
		public virtual TItem CurrentData
		{
			get { return CurrentItem; }
		}

		/// <summary>Gets the current data item.</summary>
		public new TItem CurrentItem
		{
			get { return this.currentItem; }
		}

		/// <summary>Gets whether the current data item is referenced in the query string. This usually occurs when the item is selected in edit mode.</summary>
		public bool IsHighlighted
		{
			get
			{
				if (!string.IsNullOrEmpty(Request.QueryString["item"]))
					return this.CurrentData.ID == int.Parse(Request.QueryString["item"]);
				return false;
			}
		}

		ContentItem IDataItemContainer.CurrentItem
		{
			get { return currentItem; }
			set { currentItem = (TItem)value; }
		}

		ContentItem IItemContainer.CurrentItem
		{
			get { return currentItem; }
		}
	}
}
