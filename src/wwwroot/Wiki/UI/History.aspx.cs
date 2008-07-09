using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace N2.Templates.Wiki.UI
{
    public partial class History : WikiTemplatePage
    {
        protected override void OnInit(EventArgs e)
        {
            rptArticles.DataSource = N2.Find.Items
                .Where.VersionOf.Eq(CurrentPage)
                .Select();
            DataBind();
            base.OnInit(e);
        }
    }
}
