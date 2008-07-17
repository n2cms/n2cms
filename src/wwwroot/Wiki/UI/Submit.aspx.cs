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
using System.Text.RegularExpressions;

namespace N2.Templates.Wiki.UI
{
    public partial class SubmitArticle : WikiTemplatePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        //protected void Submit_Click(object sender, EventArgs args)
        //{
        //    if (!IsAuthorized)
        //        return;

        //    HtmlFilter filter = Engine.Resolve<HtmlFilter>();
        //    Items.WikiArticle article = Engine.Definitions.CreateInstance<Items.WikiArticle>(CurrentPage);
        //    article.Title = filter.StripHtml(CurrentPage.ActionParameter);
        //    article.Name = filter.CleanUrl(CurrentPage.ActionParameter);
        //    article.Text = filter.FilterHtml(txtText.Text);
        //    Engine.Persister.Save(article);

        //    Response.Redirect(article.Url);
        //}
    }
}
