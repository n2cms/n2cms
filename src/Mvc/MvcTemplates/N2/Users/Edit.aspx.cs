using N2.Edit.Web;
using N2.Web;
using System;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace N2.Edit.Membership
{
	public partial class Edit : EditPage
	{
		string SelectedUserName;
		private MembershipUser SelectedUser;

		protected void Page_Load(object sender, EventArgs e)
		{
			LoadSelectedUser();
            hlPassword.NavigateUrl = "{ManagementUrl}/Users/Password.aspx?user=".ResolveUrlTokens() + Request["user"];
			if (!IsPostBack)
			{
				cblRoles.DataBind();
				this.txtEmail.Text = SelectedUser.Email;
				foreach (ListItem item in cblRoles.Items)
					item.Selected = Roles.IsUserInRole(SelectedUserName, item.Value);
			}
		}

		private void LoadSelectedUser()
		{
			SelectedUserName = Request.QueryString["user"];
			MembershipUserCollection muc = System.Web.Security.Membership.FindUsersByName(SelectedUserName);
			if (muc.Count < 1)
				throw new N2Exception("User '{0}' not found.", SelectedUserName);
			SelectedUser = muc[SelectedUserName];
		}

		protected void btnSave_Click(object sender, EventArgs e)
		{
			SelectedUser.Email = txtEmail.Text;
			foreach (ListItem item in cblRoles.Items)
			{
				if (item.Selected && !Roles.IsUserInRole(SelectedUserName, item.Value))
					Roles.AddUserToRole(SelectedUserName, item.Value);
				else if (!item.Selected && Roles.IsUserInRole(SelectedUserName, item.Value))
					Roles.RemoveUserFromRole(SelectedUserName, item.Value);
			}
			System.Web.Security.Membership.UpdateUser(SelectedUser);
			Response.Redirect("Users.aspx");
		}
	}
}
