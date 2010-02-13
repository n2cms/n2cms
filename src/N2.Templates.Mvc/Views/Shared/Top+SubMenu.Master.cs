using System;
using N2.Templates.Mvc.Web;
using N2.Web.Mvc;
using N2.Web.Mvc.Html;
using N2.Web.UI;

namespace N2.Templates.Mvc.Views.Shared
{
	public partial class Top_SubMenu : System.Web.Mvc.ViewMasterPage
	{
		protected string GetBodyClass()
		{
			if (CurrentItem != null)
			{
                string className = CurrentItem.GetType().Name;
				return className.Substring(0, 1).ToLower() + className.Substring(1);
			}
			return null;
		}

		protected override void OnInit(EventArgs e)
		{
			N2.Context.Current.Resolve<IPageModifierContainer>().Modify(Page);
			N2.Resources.Register.JQuery(Page);

			base.OnInit(e);
		}

		public ContentItem CurrentItem
		{
			get { return Html.CurrentPage(); }
		}
	}
}