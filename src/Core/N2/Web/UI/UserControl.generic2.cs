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
		#region Properties

		private TItem currentData;
		/// <summary>Gets the current data item.</summary>
		public virtual TItem CurrentData
		{
			get { return this.currentData ?? ItemUtility.CurrentContentItem as TItem; }
			set { this.currentData = value; }
		}

		/// <summary>Gets the current data item.</summary>
		public new TItem CurrentItem
		{
			get { return this.CurrentData; }
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
		#endregion

		#region IDataItemContainer & IItemContainer Members

		ContentItem IDataItemContainer.CurrentData
		{
			get { return this.CurrentData; }
			set { this.CurrentData = (TItem)value; }
		}
		ContentItem IItemContainer.CurrentItem
		{
			get { return this.CurrentData; }
		}

		#endregion
	}
}
