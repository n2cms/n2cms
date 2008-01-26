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

namespace N2.Edit.Membership
{
	[N2.Edit.ToolbarPlugIn("", "users", "~/Edit/Membership/Users.aspx", ToolbarArea.Preview, "preview", "~/Edit/Img/Ico/group_key.gif", 110, ToolTip = "administrate users", AuthorizedRoles = new string[] { "Administrators", "Admin" })]
	public partial class Users : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
		}

		protected void odsUsers_ItemCommand(object sender, DataGridCommandEventArgs args)
		{
			if (args.CommandName == "Delete")
			{
				System.Web.Security.Membership.DeleteUser((string)dgrUsers.DataKeys[args.Item.ItemIndex], true);
				dgrUsers.DataBind();
			}
		}

		//protected override void OnError(EventArgs e)
		//{
		//    Response.Write(Server.GetLastError());
		//    Server.ClearError();
		//}
	}
}
