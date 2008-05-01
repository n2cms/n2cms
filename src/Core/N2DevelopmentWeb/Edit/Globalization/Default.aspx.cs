using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using N2.Edit.Web;
using N2.Globalization;
using System.Collections.Generic;

namespace N2.Edit.Globalization
{
	[ToolbarPlugin("", "globalization", "~/Edit/Globalization/Default.aspx?selected={selected}", ToolbarArea.Preview, "preview", "~/Edit/Img/Ico/world.gif", 150, ToolTip = "view translations", GlobalResourceClassName = "Toolbar")]
	public partial class _Default : EditPage
	{
		protected ILanguageGateway gateway;
		protected IEnumerable<ILanguage> languages;

		protected override void OnInit(EventArgs e)
		{
			gateway = Engine.Resolve<ILanguageGateway>();
			languages = gateway.GetAvailableLanguages();

			hlCancel.NavigateUrl = SelectedNode.PreviewUrl;

			//rptLanguages.DataSource = gateway.GetEditTranslations(SelectedItem, true);
			//rptLanguages.DataSource = gateway.GetAvailableLanguages();
			DataBind();

			base.OnInit(e);
		}
	}
}
