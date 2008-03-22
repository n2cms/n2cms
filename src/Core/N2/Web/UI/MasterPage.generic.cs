using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Web.UI
{
    /// <summary>MasterPage base class providing easy access to current page item.</summary>
    /// <typeparam name="T">The type of content item for this masterpage</typeparam>
    public class MasterPage<TPage> : System.Web.UI.MasterPage, IPageItemContainer where TPage : N2.ContentItem
    {
		public virtual TPage CurrentPage
		{
			get { return (TPage)N2.Context.CurrentPage; }
		}
		public virtual TPage CurrentItem
		{
			get { return CurrentPage; }
		}

		#region IItemContainer Members

		ContentItem IPageItemContainer.CurrentPage
		{
			get { return this.CurrentPage; }
		}
		ContentItem IItemContainer.CurrentItem
		{
			get { return this.CurrentPage; }
		}

		#endregion
    }
}
