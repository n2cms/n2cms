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

namespace N2.Edit.Globalization
{
	[ToolbarPlugin("", "globalization", "~/Edit/Globalization/Default.aspx?selected={selected}", ToolbarArea.Preview, "preview", "~/Edit/Img/Ico/world.gif", 150, ToolTip = "view translations", GlobalResourceClassName = "Toolbar")]
	public partial class _Default : EditPage
	{
		protected override void OnInit(EventArgs e)
		{
			hlCancel.NavigateUrl = SelectedNode.PreviewUrl;

			rptLanguages.DataSource = Engine.Resolve<ILanguageGateway>().GetEditTranslations(SelectedItem, true);
			DataBind();

			base.OnInit(e);
		}

		protected string GetClass()
		{
			string className = (bool)Eval("IsNew") ? "new" : "existing";
			if (SelectedItem == (ContentItem)Eval("ExistingItem"))
				className += " current";
			return className;
		}
	}
}
