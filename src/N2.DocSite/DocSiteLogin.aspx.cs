using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace N2.DocSite
{
    public partial class DocSiteLogin : System.Web.UI.Page
    {
        #region Public Properties
        #endregion

        #region Private / Protected
        #endregion

        #region Constructors
        /// <summary>
        /// Constructs a new instance of the <see cref="DocSiteLogin" /> class.
        /// </summary>
        public DocSiteLogin()
        {
        }
        #endregion

        #region Methods
        #endregion

        #region Event Handlers
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ((DocSite)Master).EnsureRobotsMetaTag(true, true);
        }

        protected override void OnPreRender(EventArgs e)
        {
            Form.DefaultButton = login.FindControl("LoginButton").UniqueID;

            TextBox userName = (TextBox)login.FindControl("UserName");
            userName.Attributes.Add("onfocus", "this.select();");

            TextBox password = (TextBox)login.FindControl("Password");
            password.Attributes.Add("onfocus", "this.select();");

            if (userName.Text.Length > 0)
                password.Focus();
            else
                userName.Focus();

            base.OnPreRender(e);
        }

        protected void login_Authenticate(object sender, AuthenticateEventArgs e)
        {
            e.Authenticated = FormsAuthentication.Authenticate(login.UserName, login.Password);
        }
        #endregion
    }
}
