using System;
using System.Web.Security;
using System.Web.UI.WebControls;
using N2.Templates.Items;

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
            if (CurrentItem.LogoutPage != null)
                Response.Redirect(CurrentItem.LogoutPage.Url);
            else if (!CurrentPage.IsAuthorized(N2.Security.AuthorizedRole.AnonymousUser))
                Response.Redirect(Find.StartPage.Url);
        }

        void LoginBox_Authenticate(object sender, AuthenticateEventArgs e)
        {
            if (Membership.ValidateUser(LoginBox.UserName, LoginBox.Password))
                e.Authenticated = Membership.GetUser(LoginBox.UserName).IsApproved;
            else if (FormsAuthentication.Authenticate(LoginBox.UserName, LoginBox.Password))
                e.Authenticated = true;
        }

        void LoginBox_LoggedIn(object sender, EventArgs e)
        {
            // Get the return url
            string defaultUrl;
            if (CurrentItem == null || string.IsNullOrEmpty(CurrentItem.Url))   defaultUrl = string.Empty;
            else if (CurrentItem.LoginPage != null)                             defaultUrl = CurrentItem.LoginPage.Url ?? string.Empty;
            else
                defaultUrl = CurrentPage.Url;
            string returnUrl = defaultUrl;

            // Retrieve return url if required
            if (CurrentItem.ReturnToUrlIfPossible)
            {
                // First, retrieve the url
                returnUrl = FormsAuthentication.GetRedirectUrl(Page.User.Identity.Name, false);

                // Decode the url correctly
                int safeCounter = 25;
                while ((safeCounter > 0) && (!string.IsNullOrEmpty(returnUrl)) && (!returnUrl.StartsWith("/")))
                {
                    safeCounter--; // Decrease the counter to prevent lock ups
                    returnUrl = Server.UrlDecode(returnUrl); // Try to decode the url again
                }

                // In case the safe counter expired, use default url
                if (safeCounter <= 0) returnUrl = defaultUrl;
            }

            // Redirect
            Response.Redirect(returnUrl, true);

        }
    }
}
