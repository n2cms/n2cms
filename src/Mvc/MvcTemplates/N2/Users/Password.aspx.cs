using N2.Edit.Web;
using System;
using System.Web.Security;
using N2.Security;

namespace N2.Edit.Membership
{
    public partial class Password : EditPage
    {
        string SelectedUserName;
        private IAccountInfo SelectedUser;
        private AccountManager AccountManager { get { return N2.Context.Current.Resolve<AccountManager>(); } }

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

            SelectedUser = AccountManager.FindUserByName(SelectedUserName);
            if (SelectedUser == null)
               throw new N2Exception("User '{0}' not found.", SelectedUserName);
            
            /* REMOVE: MembershipUserCollection muc = System.Web.Security.Membership.FindUsersByName(SelectedUserName);
            if (muc.Count < 1)
                throw new N2.N2Exception("User '{0}' not found.", SelectedUserName);
            SelectedUser = muc[SelectedUserName];
            */
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (SelectedUser == null)
               throw new N2.N2Exception("User '{0}' not found.", SelectedUserName);
            bool ok = AccountManager.ChangePassword(SelectedUser.UserName, this.txtPassword.Text);

            /* REMOVE: string tempPW = SelectedUser.ResetPassword();
            bool ok = SelectedUser.ChangePassword(tempPW, this.txtPassword.Text);
            */

            if(ok)
                Response.Redirect("Users.aspx");
        }

        protected void btnUnlock_Click(object sender, EventArgs e)
        {
            if (SelectedUser == null)
                throw new N2.N2Exception("User '{0}' not found.", SelectedUserName);
            AccountManager.UnlockUser(SelectedUser.UserName);
            
            // REMOVE: SelectedUser.UnlockUser();
        }
    }
}
