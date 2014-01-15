using System;

namespace N2.Addons.Wiki.UI.WikiTemplates
{
    [DefaultWikiTemplate]
    public partial class AllArticles : WikiTemplate
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
