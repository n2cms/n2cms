using System;
using System.Web.Security;
using System.Web.UI.WebControls;
using N2.Web;
using N2.Security;
using N2.Edit.Web;

namespace N2.Edit.Membership
{
    public partial class Edit : EditPage
    {
        private string SelectedUserName;
        private IAccountInfo SelectedUser;

        private AccountManager AccountManager
        {
            get { return N2.Context.Current.Resolve<AccountManager>(); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadSelectedUser();
            hlPassword.NavigateUrl = "{ManagementUrl}/Users/Password.aspx?user=".ResolveUrlTokens() + Request["user"];
			cblRoles.Visible = AccountManager.AreRolesEnabled();
            if (IsPostBack) return;

            txtEmail.Text = SelectedUser.Email;

			if (cblRoles.Visible)
            {
				cblRoles.DataSourceID = null;
				cblRoles.DataSource = AccountManager.GetAllRoles();
                cblRoles.DataBind();
                foreach (ListItem item in cblRoles.Items)
                    item.Selected = AccountManager.IsUserInRole(SelectedUserName, item.Value);
                // REMOVE: item.Selected = Roles.IsUserInRole(SelectedUserName, item.Value);
            }
            else
            {
                cblRoles.DataSourceID = null;
                lblRoles.Visible = false;
            }
        }

        private void LoadSelectedUser()
        {
            SelectedUserName = Request.QueryString["user"];

            SelectedUser = AccountManager.FindUserByName(SelectedUserName);
            if (SelectedUser == null)
                throw new N2Exception("User '{0}' not found.", SelectedUserName);

            /* REMOVE: MembershipUserCollection muc = System.Web.Security.Membership.FindUsersByName(SelectedUserName);
            if (muc.Count < 1)
                throw new N2Exception("User '{0}' not found.", SelectedUserName);
            SelectedUser = muc[SelectedUserName];
            */
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (SelectedUser == null)
                throw new N2Exception("User '{0}' not found.", SelectedUserName);

            if (Roles.Enabled)
            {
                foreach (ListItem item in cblRoles.Items)
                {
                    if (item.Selected && !AccountManager.IsUserInRole(SelectedUserName, item.Value))
                        AccountManager.AddUserToRole(SelectedUserName, item.Value);
                    else if (!item.Selected && AccountManager.IsUserInRole(SelectedUserName, item.Value))
                        AccountManager.RemoveUserFromRole(SelectedUserName, item.Value);
                }
                AccountManager.UpdateUserEmail(SelectedUserName, txtEmail.Text);
                Response.Redirect("Users.aspx");

                /* REMOVE: SelectedUser.Email = txtEmail.Text;
            foreach (ListItem item in cblRoles.Items)
            {
                    if (item.Selected && !Roles.IsUserInRole(SelectedUserName, item.Value))
                        Roles.AddUserToRole(SelectedUserName, item.Value);
                    else if (!item.Selected && Roles.IsUserInRole(SelectedUserName, item.Value))
                        Roles.RemoveUserFromRole(SelectedUserName, item.Value);
                }
            }
            System.Web.Security.Membership.UpdateUser(SelectedUser);
            Response.Redirect("Users.aspx");
             */
            }
        }
    }

	public static class RolesSource
	{
		private static AccountManager AccountManager
		{
			get { return N2.Context.Current.Resolve<AccountManager>(); }
		}

		public static string[] GetAllRoles()
		{
			return AccountManager.GetAllRoles();
		}
	}
}
