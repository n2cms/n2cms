using System;
using N2.Web.Mvc;
using N2.Web.UI;

namespace N2.Templates.Mvc.Views.Shared
{
	public partial class Top_SubMenu : System.Web.Mvc.ViewMasterPage<IItemContainer>, IItemContainer<ContentItem>
	{
		protected string GetBodyClass()
		{
			if (Model != null)
			{
				string className = Model.GetType().Name;
				return className.Substring(0, 1).ToLower() + className.Substring(1);
			}
			return null;
		}

		/// <summary>Gets the item associated with the item container.</summary>
		ContentItem IItemContainer.CurrentItem
		{
			get { return CurrentItem; }
		}

		public ContentItem CurrentItem
		{
			get
			{
				return Model.CurrentItem;
			}
		}
	}
}