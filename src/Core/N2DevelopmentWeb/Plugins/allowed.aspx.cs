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

namespace N2.TemplateWeb.Plugins
{
	[N2.Edit.ToolbarPlugin("allowed", "allowed", "/plugins/allowed.aspx", N2.Edit.ToolbarArea.Preview, GlobalResourceClassName = "N2Dev")]
	[N2.Edit.ToolbarPlugin("localized", "localized", "/plugins/allowed.aspx", N2.Edit.ToolbarArea.Preview, GlobalResourceClassName = "N2Dev")]
	[N2.Edit.PlugInAuthorizedRolesAttribute("admin")]
	public partial class allowed : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}
	}
}
