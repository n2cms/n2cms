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

namespace N2.Templates.UI
{
	public partial class Redirect : Web.UI.TemplatePage<Items.Redirect>
	{
		protected override void OnInit(EventArgs e)
		{
			if (CurrentPage.Redirect301)
			{
				Response.Status = "301 Moved Permanently";
				Response.AddHeader("Location", CurrentPage.Url);
			}
			else
			{
				Response.Redirect(CurrentPage.Url);
			}
		}
	}
}
