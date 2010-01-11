using System;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Edit.Web;

namespace N2.Edit
{
	public partial class Login : EditPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			Login1.Focus();

			if (Request.QueryString["logout"] == null) 
				return;

			FormsAuthentication.SignOut();
            Response.Redirect("login.aspx?returnUrl=default.aspx");
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
				if (FormsAuthentication.Authenticate(Login1.UserName, Login1.Password))
				{
					e.Authenticated = true;
					FormsAuthentication.RedirectFromLoginPage(Login1.UserName, Login1.RememberMeSet);
				}
				else if (System.Web.Security.Membership.ValidateUser(Login1.UserName, Login1.Password))
				{
					e.Authenticated = true;
					FormsAuthentication.RedirectFromLoginPage(Login1.UserName, Login1.RememberMeSet);
				}
			}
			catch (Exception ex)
			{
				Trace.Warn(ex.ToString());
				e.Authenticated = false;
			}
		}
	}
}
