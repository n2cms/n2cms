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

namespace N2.Templates.Wiki.UI.WikiTemplates
{
    [WikiTemplate("~/Wiki/UI/WikiTemplates/AllArticles.ascx")]
    public partial class AllArticles : WikiUserControl<Items.WikiArticle>
    {
        protected override void OnInit(EventArgs e)
        {
            rptArticles.DataSource = N2.Find.Items
                .Where.Type.Eq(typeof(Items.WikiArticle))
                .And.Parent.Eq((ContentItem)Viewed.Article.WikiRoot)
                .OrderBy.Title.Asc.Select();

            DataBind();

            base.OnInit(e);
        }
    }
}