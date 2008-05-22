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
	public partial class New : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
		}

		protected void createUserWizard_CreatedUser(object sender, EventArgs e)
		{
			CheckBoxList cblRoles = (CheckBoxList)cuwsCreate.Controls[0].FindControl("cblRoles");
			foreach (ListItem item in cblRoles.Items)
				if (item.Selected)
					Roles.AddUserToRole(createUserWizard.UserName, item.Value);
		}

		protected void createUserWizard_FinishButtonClick(object sender, EventArgs e)
		{
			Response.Redirect("Users.aspx");
		}
	}
}
