using System;
using N2.Persistence;
using N2.Edit.Versioning;

namespace N2.Addons.Wiki.UI.Views
{
    public partial class History : WikiTemplatePage
    {
        protected override void OnInit(EventArgs e)
        {
            rptArticles.DataSource = Engine.Resolve<IVersionManager>().GetVersionsOf(CurrentPage);
                //N2.Find.Items
                //.Where.VersionOf.Eq(CurrentPage)
                //.Select();
            DataBind();
            base.OnInit(e);
        }
    }
}
