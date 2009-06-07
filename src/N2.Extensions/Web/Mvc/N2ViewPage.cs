using System;
using System.Web.Mvc;
using N2.Web.UI;

namespace N2.Web.Mvc
{
	/// <summary>
	/// A ViewPage implementation that allows N2 Display helpers to be used
	/// 
	/// The Model must be a ContentItem
	/// </summary>
	/// <typeparam name="TItem"></typeparam>
	public class N2ViewPage<TItem> : ViewPage<TItem>, IItemContainer<TItem>
		where TItem : ContentItem
	{
		public TItem CurrentItem
		{
			get { return Model; }
		}

		ContentItem IItemContainer.CurrentItem
		{
			get { return CurrentItem; }
		}
	}
}