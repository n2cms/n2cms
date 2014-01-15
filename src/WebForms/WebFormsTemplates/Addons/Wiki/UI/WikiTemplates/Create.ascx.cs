using System;
using N2.Web;

namespace N2.Addons.Wiki.UI.WikiTemplates
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
