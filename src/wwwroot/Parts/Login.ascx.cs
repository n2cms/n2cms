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
using N2.Templates.UI.Items.Parts;

namespace N2.Templates.UI.Parts
{
	public partial class Login : Web.UI.TemplateUserControl<ContentItem, LoginItem>
	{
		protected override void OnInit(EventArgs e)
		{
			LoginBox.Authenticate += new AuthenticateEventHandler(LoginBox_Authenticate);
			LoginBox.LoggedIn += new EventHandler(LoginBox_LoggedIn);
			Status.LoggedOut += new EventHandler(Status_LoggedOut);
			base.OnInit(e);
		}

		void Status_LoggedOut(object sender, EventArgs e)
		{
			Response.Redirect((CurrentItem.LogoutPage ?? Find.StartPage).Url);
		}

		void LoginBox_Authenticate(object sender, AuthenticateEventArgs e)
		{
			e.Authenticated = Membership.ValidateUser(LoginBox.UserName, LoginBox.Password);
		}

		void LoginBox_LoggedIn(object sender, EventArgs e)
		{
			if (CurrentItem.LoginPage != null)
				Response.Redirect(CurrentItem.LoginPage.Url);
		}
	}
}