using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace N2.Edit
{
    public partial class Login : Web.EditPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
			this.Login1.Focus();
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
