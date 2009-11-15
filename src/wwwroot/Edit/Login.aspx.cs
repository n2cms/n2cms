using System;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Edit.Web;

namespace N2.Edit
{
	[LogoutToolbarPlugin("", "logout", "", 10, ToolTip = "logout", GlobalResourceClassName = "Toolbar")]
	public partial class Login : EditPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			Login1.Focus();

			if (Request.QueryString["logout"] == null) 
				return;

			FormsAuthentication.SignOut();
			FormsAuthentication.RedirectToLoginPage();
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

	public class LogoutToolbarPluginAttribute : ToolbarPluginAttribute
	{
		public LogoutToolbarPluginAttribute(string title, string name, string target, int sortOrder)
			: base(title, name, "", ToolbarArea.Search, target, "", sortOrder)
		{
		}

		public override Control AddTo(Control container, PluginContext context)
		{
			var link = new HyperLink();

			link.ID = "logout";
			link.SkinID = "Logout";
			link.NavigateUrl = "Login.aspx?logout=true";
			link.Text = "Log out of Admin";

			container.Controls.Add(link);

			return link;
		}
	}
}
