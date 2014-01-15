using System;
using System.Collections.Generic;
using N2.Collections;

namespace N2.Addons.Wiki.UI.Views
{
    public partial class SearchArticle : WikiTemplatePage
    {
        protected override void OnInit(EventArgs e)
        {
            IList<ContentItem> hits = N2.Find.Items
                .Where.Parent.Eq((ContentItem)CurrentPage.WikiRoot)
                .And.OpenBracket()
                .Title.Like("%" + CurrentArguments + "%")
                .Or.Detail("Text").Like("%" + CurrentArguments + "%")
                .CloseBracket()
                .Filters(new AccessFilter(Page.User, Engine.SecurityManager), new PageFilter())
                .Select();
            
            if (hits.Count == 0)
                Response.Redirect(new N2.Web.Url(CurrentPage.Url).AppendSegment("nohits").AppendSegment(CurrentArguments));

            rptArticles.DataSource = hits;
            DataBind(); 
            
            base.OnInit(e);
        }
    }
}
