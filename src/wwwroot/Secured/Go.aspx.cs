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
using N2.Edit;

namespace N2.Templates.UI.Secured
{
    [ToolbarPlugin("", "", "~/Secured/Go.aspx?selected={selected}", N2.Edit.ToolbarArea.Preview, "_top", "~/Img/eye.png", 0, Name = "Go")]
    public partial class Go : N2.Edit.Web.EditPage
	{
		protected override void OnInit(EventArgs e)
		{
			Response.Redirect(SelectedItem.Url);
		}
	}
}
