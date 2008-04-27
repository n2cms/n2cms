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

namespace N2DevelopmentWeb.Plugins
{
	[N2.Edit.ToolbarPlugin("unallowed", "unallowed", "/plugins/unallowed.aspx", N2.Edit.ToolbarArea.Preview)]
	[N2.Edit.PlugInAuthorizedRoles("NonExistantRole")]
	public partial class unallowed : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}
	}
}
