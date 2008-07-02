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
    public partial class SubmitArticle : Templates.Web.UI.TemplatePage<Items.Wiki>
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Submit_Click(object sender, EventArgs args)
        {
            HtmlFilter filter = Engine.Resolve<HtmlFilter>();
            Items.WikiArticle article = Engine.Definitions.CreateInstance<Items.WikiArticle>(CurrentPage);
            string title = filter.StripHtml(txtTitle.Text);
            article.Title = title;
            article.Name = Regex.Replace(title, "[ ?&:<>*/]*", "-");
            article.Text = filter.FilterHtml(txtText.Text);
            Engine.Persister.Save(article);

            Response.Redirect(article.Url);
        }
    }
}
