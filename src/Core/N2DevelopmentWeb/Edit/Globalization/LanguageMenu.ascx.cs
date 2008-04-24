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
using N2.Edit.Web;
using N2.Globalization;

namespace N2.Edit.Globalization
{
	[EditToolbarPlugin("~/Edit/Globalization/LanguageMenu.ascx")]
	public partial class LanguageMenu : EditUserControl
	{
		protected Repeater rptLanguages;

		protected override void OnInit(EventArgs e)
		{
			rptLanguages.DataSource = Engine.Resolve<ILanguageGateway>().GetLanguages();
			DataBind();
			this.Visible = rptLanguages.Items.Count > 0;

			base.OnInit(e);
		}
	}
}