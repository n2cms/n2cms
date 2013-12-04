using System;
using System.Web.UI.WebControls;
using N2.Web;
using N2.Edit.Web;
using N2.Security;

namespace N2.Edit.Membership
{
	public partial class Edit : EditPage
	{
		string SelectedUserName;
        private IAccountInfo SelectedUser;

        private AccountManager AccountManager { get { return N2.Context.Current.Resolve<AccountManager>(); } }

		protected void Page_Load(object sender, EventArgs e)
		{
			LoadSelectedUser();
            hlPassword.NavigateUrl = "{ManagementUrl}/Users/Password.aspx?user=".ResolveUrlTokens() + Request["user"];
			if (!IsPostBack)
			{
				cblRoles.DataBind();
				this.txtEmail.Text = SelectedUser.Email;
				foreach (ListItem item in cblRoles.Items)
                    item.Selected = AccountManager.IsUserInRole(SelectedUserName, item.Value);
			}
		}

		private void LoadSelectedUser()
		{
			SelectedUserName = Request.QueryString["user"];
            SelectedUser = AccountManager.FindUserByName(SelectedUserName);
	        if (SelectedUser == null)
				throw new N2Exception("User '{0}' not found.", SelectedUserName);
		}

		protected void btnSave_Click(object sender, EventArgs e)
		{
            if (SelectedUser == null)
                throw new N2Exception("User '{0}' not found.", SelectedUserName);

			foreach (ListItem item in cblRoles.Items)
			{
                if (item.Selected && !AccountManager.IsUserInRole(SelectedUserName, item.Value))
                    AccountManager.AddUserToRole(SelectedUserName, item.Value);
                else if (!item.Selected && AccountManager.IsUserInRole(SelectedUserName, item.Value))
                    AccountManager.RemoveUserFromRole(SelectedUserName, item.Value);
			}
            AccountManager.UpdateUserEmail(SelectedUserName, txtEmail.Text);
			Response.Redirect("Users.aspx");
		}
	}


    public static class RolesSource
    {
        private static AccountManager AccountManager { get { return N2.Context.Current.Resolve<AccountManager>(); } }

        public static string[] GetAllRoles()
        {
            return AccountManager.GetAllRoles();
        }
        
    }

}
