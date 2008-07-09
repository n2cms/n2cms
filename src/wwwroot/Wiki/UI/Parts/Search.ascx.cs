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
using N2.Templates.Wiki.Web;
using N2.Web;

namespace N2.Templates.Wiki.UI.Parts
{
    [WikiTemplate("~/Wiki/UI/Parts/Search.ascx")]
    public partial class Search : WikiUserControl<Items.WikiArticle>
    {
        protected override void OnInit(EventArgs e)
        {
            if (CurrentPage.Action == "search")
            {
                txtSearch.Text = CurrentPage.ActionParameter;
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