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
using N2.Templates.Services;
using N2.Templates.Wiki.Web;
using N2.Persistence;
using N2.Templates.Configuration;
using System.Web.Configuration;
using N2.Resources;

namespace N2.Templates.Wiki.UI.Parts
{
    public partial class EditArticle : WikiUserControl<Items.WikiArticle>
    {
        public bool IsNew { get; set; }
        public string Text { get; set; }

        protected override void OnInit(EventArgs e)
        {
            if (IsNew)
            {
                h1.Text = CurrentArguments;
            }
            else
            {
                h1.Text = CurrentPage.Title;
                txtText.Text = CurrentPage.Text;
            }
            phSubmit.Visible = cvAuthorized.IsValid = IsAuthorized;
            if (!string.IsNullOrEmpty(Text))
            {
                WikiParser parser = Engine.Resolve<WikiParser>();
                WikiRenderer renderer = Engine.Resolve<WikiRenderer>();
                renderer.AddTo(parser.Parse(Text), pnlMessage, CurrentPage);
            }

            TemplatesSection config = WebConfigurationManager.GetSection("n2/templates") as TemplatesSection;
            txtText.EnableFreeTextArea = config != null && config.Wiki.FreeTextMode;

            Register.JQuery(Page);

            base.OnInit(e);
        }

        protected void Submit_Click(object sender, EventArgs args)
        {
            if (!IsAuthorized)
            {
                cvAuthorized.IsValid = false;
                return;
            }

            HtmlFilter filter = Engine.Resolve<HtmlFilter>();
            Items.WikiArticle article = CurrentPage;
            if (IsNew)
            {
                article = Engine.Definitions.CreateInstance<Items.WikiArticle>(CurrentPage);
                article.Title = filter.StripHtml(CurrentArguments);
				article.Name = filter.CleanUrl(CurrentArguments);
            }
            else
            {
                Engine.Resolve<IVersionManager>().SaveVersion(article);
            }
            article["SavedDate"] = DateTime.Now;
            article["SavedByAddress"] = Request.UserHostAddress;
            article[SyndicatableDefinitionAppender.SyndicatableDetailName] = CurrentPage.WikiRoot[SyndicatableDefinitionAppender.SyndicatableDetailName];
            article.Text = filter.FilterHtml(txtText.Text);
            Engine.Persister.Save(article);

            Response.Redirect(article.Url);
        }
    }
}