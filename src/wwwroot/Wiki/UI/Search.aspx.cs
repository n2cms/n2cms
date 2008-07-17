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
using System.Collections.Generic;

namespace N2.Templates.Wiki.UI
{
    public partial class SearchArticle : WikiTemplatePage
    {
        protected override void OnInit(EventArgs e)
        {
            IList<ContentItem> hits = N2.Find.Items
                .Where.Parent.Eq((ContentItem)CurrentPage.WikiRoot)
                .And.OpenBracket().Title.Like("%" + CurrentPage.ActionParameter + "%")
                .Or.Detail("Text").Like("%" + CurrentPage.ActionParameter + "%").CloseBracket()
                .Select();
            
            if (hits.Count == 0)
                Response.Redirect(new N2.Web.Url(CurrentPage.Url).AppendSegment("nohits").AppendSegment(CurrentPage.ActionParameter));

            rptArticles.DataSource = hits;
            DataBind(); 
            
            base.OnInit(e);
        }
    }
}
