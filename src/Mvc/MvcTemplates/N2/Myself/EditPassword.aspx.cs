using System;
using System.Web;
using System.Web.Security;
using N2.Edit.Web;
using System.Drawing;


namespace N2.Edit.Myself
{
    public partial class EditPassword : EditPage
    {
        string SelectedUserName;
        private MembershipUser SelectedUser;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!LoadSelectedUser())
            {
                cvNoUser.IsValid = false;
                btnSave.Enabled = false;
                txtPassword.Enabled = false;
                txtOldPassword.Enabled = false;
                txtRepeatPassword.Enabled = false;
            }
            else
                btnSave.Enabled = !SelectedUser.IsLockedOut;

        }

        private bool LoadSelectedUser()
        {
            SelectedUserName = HttpContext.Current.User.Identity.Name;
            MembershipUserCollection muc = System.Web.Security.Membership.FindUsersByName(SelectedUserName);
            if (muc.Count < 1)
                return false;
            SelectedUser = muc[SelectedUserName];
            return true;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            lblStatusReset.Text = "";
            lblStusRepeatPassword.Text = "";
            lblStatusReset.ForeColor = lblStusRepeatPassword.ForeColor = Color.Red;

            var approved = OldPasswordApproval && txtPassword.Text.Equals(txtRepeatPassword.Text) && string.IsNullOrWhiteSpace(txtPassword.Text) == false;

            if (OldPasswordApproval == false)
                lblStatusReset.Text = "Your old password does not match";

            if (txtPassword.Text.Equals(txtRepeatPassword.Text) == false || approved == false)
                lblStusRepeatPassword.Text = "Repeated password does not match with your 'New Passoword'";

            if (approved) ResetPassword();
        }

        private void ResetPassword()
        {
            string tempPW = SelectedUser.ResetPassword();
            bool ok = SelectedUser.ChangePassword(tempPW, this.txtPassword.Text);
            if (ok)
            {
                lblStatusReset.ForeColor = Color.DarkGreen;
                lblStatusReset.Text = "Your password has been changed";
            }

            else lblStatusReset.Text = "Something went wrong!";
        }

        private bool OldPasswordApproval
        {
            get { return System.Web.Security.Membership.ValidateUser(SelectedUserName, txtOldPassword.Text); }
        }
    }
}
