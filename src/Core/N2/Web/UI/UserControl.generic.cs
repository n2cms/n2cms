using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace N2.Web.UI
{
	/// <summary>A user control base used to for quick access to content data.</summary>
	/// <typeparam name="TPage">The type of page item this user control will have to deal with.</typeparam>
	public class UserControl<TPage> : UserControl, IPageItemContainer
		where TPage : N2.ContentItem
	{
		/// <summary>Gets the current CMS Engine.</summary>
		public N2.Engine.IEngine Engine
		{
			get { return N2.Context.Current; }
		}

		private TPage currentPage = null;
		/// <summary>Gets the current page item.</summary>
		public virtual TPage CurrentPage
		{
			get 
			{
				if (currentPage == null)
					currentPage = (TPage)ItemUtility.FindCurrentItem(this.Parent) ?? (TPage)N2.Context.CurrentPage;
				return (TPage)N2.Context.CurrentPage; 
			}
		}

		/// <summary>Gets the current page item.</summary>
		public TPage CurrentItem
		{
			get { return this.CurrentPage; }
		}
	
		ContentItem IPageItemContainer.CurrentPage
		{
			get { return this.CurrentPage; }
		}

		ContentItem IItemContainer.CurrentItem
		{
			get { return this.CurrentItem; }
		}
	}
}
