using System;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace N2.Edit
{
	public partial class Login : Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			Login1.Focus();

			if (Request.QueryString["logout"] == null) 
				return;

			FormsAuthentication.SignOut();
            Response.Redirect("login.aspx?returnUrl=Default.aspx");
		}

		protected void Login1_LoggingIn(object sender, LoginCancelEventArgs e)
		{
		}

		protected void Login1_LoginError(object sender, EventArgs e)
		{
		}

		protected void Login1_Authenticate(object sender, AuthenticateEventArgs e)
		{
			try
			{
				if (FormsAuthentication.Authenticate(Login1.UserName, Login1.Password)
					|| (System.Web.Security.Membership.ValidateUser(Login1.UserName, Login1.Password) && System.Web.Security.Membership.GetUser(Login1.UserName).IsApproved))
				{
					e.Authenticated = true;
					//Travis Pettijohn - Oct 2010 - pettijohn.com
					//Using FormsAuthentication.RedirectFromLoginPage crashes the Azure dev fabric load balancer (dfloadbalancer.exe).
					//Switching up the logic to set the cookie and redirect here with endResponse set to TRUE fixes the glitch. 

					//FormsAuthentication.RedirectFromLoginPage(Login1.UserName, Login1.RememberMeSet);
					FormsAuthentication.SetAuthCookie(Login1.UserName, Login1.RememberMeSet);
					string returnUrl = FormsAuthentication.GetRedirectUrl(Login1.UserName, Login1.RememberMeSet);
					Response.Redirect(returnUrl, true);
				}
			}
			catch (Exception ex)
			{
				Engine.Logger.Warn(ex);
				e.Authenticated = false;
			}
		}
	}
}
