using System.Web.UI.WebControls;
using N2.Edit.Web;
using N2.Security;

namespace N2.Edit.Membership
{
	[MembershipToolbarPlugin("USERS", "users", "{ManagementUrl}/Users/Users.aspx", "{ManagementUrl}/Resources/icons/group_key.png", 110,
		ToolTip = "administer users",
		GlobalResourceClassName = "Toolbar",
		RequiredPermission = Permission.Administer)]
	public partial class Users : EditPage
	{
		protected void odsUsers_ItemCommand(object sender, DataGridCommandEventArgs args)
		{
			if (args.CommandName == "Delete")
			{
				System.Web.Security.Membership.DeleteUser((string)dgrUsers.DataKeys[args.Item.ItemIndex], true);
				dgrUsers.DataBind();
			}
        }
	}
}
