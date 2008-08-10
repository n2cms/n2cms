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
using N2.Web;

namespace N2.Templates.Wiki.UI.WikiTemplates
{
    [DefaultWikiTemplate]
    public partial class Create : WikiTemplate
    {
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            HtmlFilter filter = Engine.Resolve<HtmlFilter>();
            string name = filter.CleanUrl(txtName.Text);
            string url = Url.Parse(CurrentPage.WikiRoot.Url).AppendSegment(name);
            Response.Redirect(url);
        }
    }
}