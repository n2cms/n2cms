using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using N2.Web.Mail;

namespace N2.Templates.UI.Parts
{
	public partial class Register : Templates.Web.UI.TemplateUserControl<ContentItem, Items.Parts.UserRegistration>
	{
		protected override void OnInit(EventArgs e)
		{
			if (Request["verify"] != null)
			{
				VerifyUser(Request["verify"]);
			}

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

				Engine.Resolve<IMailSender>().Send(CurrentItem.VerificationSender, UserCreator.Email, subject, body);
			}

			if (CurrentItem.SuccessPage != null)
			{
				Response.Redirect(CurrentItem.SuccessPage.Url);
			} 
		}
	}
}