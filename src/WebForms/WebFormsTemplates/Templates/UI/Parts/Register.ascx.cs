using System;
using System.Web.Security;
using N2.Web;
using N2.Web.Mail;
using UserRegistration=N2.Templates.Items.UserRegistration;
using N2.Configuration;
using System.Security.Principal;

namespace N2.Templates.UI.Parts
{
    public partial class Register : Templates.Web.UI.TemplateUserControl<ContentItem, UserRegistration>
    {
        protected override void OnInit(EventArgs e)
        {
            if (Request["verify"] != null)
            {
                VerifyUser(Request["verify"]);
            }

            UserCreator.CreatingUser += new System.Web.UI.WebControls.LoginCancelEventHandler(UserCreator_CreatingUser);
            UserCreator.CreatedUser += new EventHandler(UserCreator_CreatedUser);

            base.OnInit(e);
        }

        private void VerifyUser(string encryptedTicket)
        {
            FormsAuthenticationTicket t = FormsAuthentication.Decrypt(encryptedTicket);
            MembershipUser user = Membership.GetUser(t.Name);
            user.IsApproved = true;
            Membership.UpdateUser(user);

            if(CurrentItem.VerifiedPage != null)
                Response.Redirect(CurrentItem.VerifiedPage.Url);
        }

        void UserCreator_CreatingUser(object sender, System.Web.UI.WebControls.LoginCancelEventArgs e)
        {
            if (IsEditorOrAdmin(UserCreator.UserName))
            {
                cvError.IsValid = false;
                cvError.ErrorMessage = "Invalid user name.";
                e.Cancel = true;
            }
        }

        void UserCreator_CreatedUser(object sender, EventArgs e)
        {
            string un = UserCreator.UserName;
            string pw = UserCreator.Password;

            Roles.AddUserToRoles(un, CurrentItem.Roles.ToArray<string>());
            MembershipUser user = Membership.GetUser(un);

            if (CurrentItem.RequireVerification)
            {
                user.IsApproved = false;
                Membership.UpdateUser(user);

                string port = (Request.Url.Port != 80) ? (":" + Request.Url.Port) : string.Empty;

                string crypto = FormsAuthentication.Encrypt(new FormsAuthenticationTicket(un, true, 60 * 24 * 7));
                string url = Request.Url.Scheme + Uri.SchemeDelimiter + Request.Url.Host + port + CurrentPage.Url + "?verify=" + crypto;

                string subject = CurrentItem.VerificationSubject;
                string body = CurrentItem.VerificationText.Replace("{VerificationUrl}", url);

                try
                {
                    Engine.Resolve<IMailSender>().Send(CurrentItem.VerificationSender, UserCreator.Email, subject, body);

                    if (CurrentItem.SuccessPage != null)
                    {
                        Response.Redirect(CurrentItem.SuccessPage.Url);
                    }
                }
                catch (InvalidOperationException ex)
                {
                    cvError.Text = ex.Message;
                    cvError.IsValid = false;
                    UserCreator.Visible = false;
                    Engine.Resolve<IErrorNotifier>().Notify(ex);
                }
            }
            else if (CurrentItem.SuccessPage != null)
            {
                Response.Redirect(CurrentItem.SuccessPage.Url);
            } 
        }

        private bool IsEditorOrAdmin(string username)
        {
            GenericPrincipal user = new GenericPrincipal(new GenericIdentity(username), new string[0]);
            return Engine.SecurityManager.IsEditor(user) || Engine.SecurityManager.IsAdmin(user);
        }
    }
}
