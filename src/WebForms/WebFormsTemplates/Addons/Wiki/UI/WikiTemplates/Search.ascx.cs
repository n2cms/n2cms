using System;
using N2.Web;

namespace N2.Addons.Wiki.UI.WikiTemplates
{
    [DefaultWikiTemplate]
    public partial class Search : WikiTemplate
    {
        protected override void OnInit(EventArgs e)
        {
            
            if (CurrentAction == "search" || CurrentAction == "nohits")
            {
                txtSearch.Text = CurrentArguments;
            }
            base.OnInit(e);
        }
        protected override void OnPreRender(EventArgs e)
        {
            if (txtSearch.Text.Length > 0)
            {
                txtSearch.Focus();
            } 
            base.OnPreRender(e);
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            var filter = Engine.Resolve<HtmlFilter>();
            string text = filter.CleanUrl(txtSearch.Text);
            Url url = Viewed.Article.WikiRoot.Url;
            url = url.AppendSegment("search").AppendSegment(text);
            Response.Redirect(url);
        }
    }
}
