using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using LanguageSwitcher;

public partial class Languaging_LanguageSwitcher : N2.Web.UI.UserControl<PageItem, LanguageSwitcherItem>
{
	string languageParameter = LanguageSwitcherModule.getParameter4Switching();

    protected void Page_Load(object sender, EventArgs e)
    {
		if (!Page.IsPostBack)
		{
			this.ddlLanguages.DataSource = this.CurrentItem.Languages.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
			this.ddlLanguages.DataBind();
			if (LanguageSwitcherModule.RequestLanguage != null)
				this.ddlLanguages.SelectedValue = LanguageSwitcherModule.RequestLanguage;
		}
    }

	protected void ddlLanguages_SelectedIndexChanged(object sender, EventArgs e)
	{
		string url = LanguageSwitcherModule.decorateUrlForPersistentLanguageChange(Request.RawUrl, ddlLanguages.SelectedValue);
		Response.Redirect(url);
	}
}
