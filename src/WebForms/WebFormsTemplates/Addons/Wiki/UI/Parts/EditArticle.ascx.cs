using System;
using N2.Addons.Wiki.Web;
using N2.Persistence;
using N2.Resources;
using N2.Web;
using N2.Web.Wiki;
using N2.Edit.Versioning;

namespace N2.Addons.Wiki.UI.Parts
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
            txtText.EnableFreeTextArea = CurrentPage.WikiRoot.EnableFreeText;
            phSubmit.Visible = cvAuthorized.IsValid = IsAuthorized;
            if (!string.IsNullOrEmpty(Text))
            {
                WikiParser parser = Engine.Resolve<WikiParser>();
                WikiRenderer renderer = Engine.Resolve<WikiRenderer>();
                renderer.AddTo(parser.Parse(Text), pnlMessage, CurrentPage);
            }

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
                article = Engine.Resolve<ContentActivator>().CreateInstance<Items.WikiArticle>(CurrentPage);
                article.Title = filter.StripHtml(CurrentArguments);
                article.Name = filter.CleanUrl(CurrentArguments);
            }
            else
            {
                Engine.Resolve<IVersionManager>().AddVersion(article);
            }
            article["SavedDate"] = DateTime.Now;
            article["SavedByAddress"] = Request.UserHostAddress;
            article["Syndicatable"] = CurrentPage.WikiRoot["Syndicatable"];
            article.Text = filter.FilterHtml(txtText.Text);
            Engine.Persister.Save(article);

            Response.Redirect(article.Url);
        }
    }
}
