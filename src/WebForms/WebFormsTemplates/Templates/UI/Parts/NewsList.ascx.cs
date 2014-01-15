using System;
using N2.Web.UI.WebControls;

namespace N2.Templates.UI.Parts
{
    public partial class NewsList : Web.UI.TemplateUserControl<ContentItem, Items.NewsList>
    {
        protected ItemDataSource idsNews;

        protected override void OnInit(EventArgs e)
        {
            idsNews.CurrentItem = CurrentItem.Container;
            idsNews.Filtering += idsNews_Filtering;
            base.OnInit(e);
        }

        void idsNews_Filtering(object sender, N2.Collections.ItemListEventArgs e)
        {
            CurrentItem.Filter(e.Items);
        }
    }
}
