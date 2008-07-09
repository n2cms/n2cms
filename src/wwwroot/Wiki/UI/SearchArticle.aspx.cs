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
    public partial class SearchArticle : WikiTemplatePage
    {
        protected override void OnInit(EventArgs e)
        {
            rptArticles.DataSource = N2.Find.Items
                .Where.Parent.Eq((ContentItem)CurrentPage.WikiRoot)
                .And.OpenBracket().Title.Like("%" + CurrentPage.ActionParameter + "%")
                .Or.Detail("Text").Like("%" + CurrentPage.ActionParameter + "%").CloseBracket()
                .Select();

            DataBind(); 
            
            base.OnInit(e);
        }
    }
}
