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
	[N2.Edit.ToolbarPlugin("", "users", "~/Edit/Membership/Users.aspx", ToolbarArea.Preview, "preview", "~/Edit/Img/Ico/group_key.gif", 110, ToolTip = "administer users", AuthorizedRoles = new string[] { "Administrators", "Admin" }, GlobalResourceClassName = "Toolbar")]
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

        protected override void OnError(EventArgs e)
        {
            string html = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<html>
    <head>
	    <link rel=""stylesheet"" href=""../Css/All.css"" type=""text/css"" />
        <link rel=""stylesheet"" href=""../Css/Framed.css"" type=""text/css"" />
    </head>
    <body><div class='content'><h1>This feature might not have been enabled in web.config. Please look into ASP.NET membership configuration.</h1><p><i>Check &lt;configuration&gt; &lt;system.web&gt; &lt;membership&gt; ... in web.config</i></p><pre><h3>" 
                + Server.GetLastError().Message 
                + "</h3>" 
                + Server.GetLastError().ToString() 
                + "</pre></body></html>";
            Response.Write(html);
            Server.ClearError();
        }
	}
}
