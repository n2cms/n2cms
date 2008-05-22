using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Web.UI
{
    /// <summary>MasterPage base class providing easy access to current page item.</summary>
    /// <typeparam name="T">The type of content item for this masterpage</typeparam>
    public abstract class MasterPage<TPage> : System.Web.UI.MasterPage, IItemContainer 
		where TPage : N2.ContentItem
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

		ContentItem IItemContainer.CurrentItem
		{
			get { return this.CurrentPage; }
		}

		#endregion
    }
}
