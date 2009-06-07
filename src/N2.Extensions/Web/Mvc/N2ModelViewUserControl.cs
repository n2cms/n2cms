using System;
using System.Web.Mvc;
using N2.Web.UI;

namespace N2.Web.Mvc
{
	/// <summary>
	/// A ViewUserControl implementation that allows N2 Display helpers to be used
	/// 
	/// This class only restricts the model by enforcing that it implements the lightweight <see cref="IItemContainer{TItem}" /> interface.
	/// This way a Model needn't be an N2 ContentItem
	/// </summary>
	/// <typeparam name="TItem"></typeparam>
	public class N2ModelViewUserControl<TContainer, TItem> : ViewUserControl<TContainer>, IItemContainer<TItem>
		where TContainer : class, IItemContainer<TItem>
		where TItem : ContentItem
	{
		public TItem CurrentItem
		{
			get { return Model.CurrentItem; }
		}

		ContentItem IItemContainer.CurrentItem
		{
			get { return CurrentItem; }
		}
	}
}