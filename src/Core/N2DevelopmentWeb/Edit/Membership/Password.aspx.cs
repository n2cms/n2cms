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
	public partial class Password : System.Web.UI.Page
	{
		string SelectedUserName;
		private MembershipUser SelectedUser;

		protected void Page_Load(object sender, EventArgs e)
		{
			LoadSelectedUser();
		}
		protected void Page_PreRender(object sender, EventArgs e)
		{
			btnUnlock.Enabled = SelectedUser.IsLockedOut;
			btnSave.Enabled = !SelectedUser.IsLockedOut;
		}

		private void LoadSelectedUser()
		{
			SelectedUserName = Request.QueryString["user"];
			MembershipUserCollection muc = System.Web.Security.Membership.FindUsersByName(SelectedUserName);
			if (muc.Count < 1)
				throw new N2.N2Exception("User '{0}' not found.", SelectedUserName);
			SelectedUser = muc[SelectedUserName];
		}

		protected void btnSave_Click(object sender, EventArgs e)
		{
			string tempPW = SelectedUser.ResetPassword();
			bool ok = SelectedUser.ChangePassword(tempPW, this.txtPassword.Text);
			if(ok)
				Response.Redirect("Users.aspx");
		}

		protected void btnUnlock_Click(object sender, EventArgs e)
		{
			SelectedUser.UnlockUser();
		}
	}
}
