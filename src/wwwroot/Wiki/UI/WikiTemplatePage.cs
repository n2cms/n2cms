using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using N2.Templates.Web.UI;

namespace N2.Templates.Wiki.UI
{
    public class WikiTemplatePage : TemplatePage<Items.WikiArticle>
    {
        protected bool IsAuthorized
        {
            get
            {
                foreach (string role in CurrentPage.WikiRoot.ModifyRoles)
                    if (User.IsInRole(role))
                        return true;
                return false;
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            N2.Resources.Register.StyleSheet(this, "~/Wiki/UI/Wiki.css");
            base.OnPreRender(e);
        }
    }
}
