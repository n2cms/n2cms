using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using N2.Templates.Items;
using N2.Globalization;
using N2.Templates.Web.UI.WebControls;

namespace N2.Templates.UI.Layouts
{
    public partial class Top_SubMenu : Templates.Web.UI.TemplateMasterPage<ContentItem>
	{
		protected ILanguageGateway languages;

		protected override void OnInit(EventArgs e)
		{
			languages = N2.Context.Current.Resolve<ILanguageGateway>();

            ContentItem language = languages.GetLanguage(CurrentPage) as ContentItem;

			if (p != null) p.Visible = N2.Templates.Find.ClosestStartPage.ShowBreadcrumb;
			if (dti != null) dti.Visible = CurrentPage["ShowTitle"] != null && (bool)CurrentPage["ShowTitle"];
			if (dh != null) dh.CurrentItem = language;

			if (zsl != null)
			{
				zsl.CurrentItem = language;
				dft.CurrentItem = language;
			}

			base.OnInit(e);
		}

		protected string GetBodyClass()
		{
			if (CurrentPage != null)
			{
				string className = CurrentPage.GetType().Name;
				return className.Substring(0, 1).ToLower() + className.Substring(1);
			}
			return null;
		}
	}
}
