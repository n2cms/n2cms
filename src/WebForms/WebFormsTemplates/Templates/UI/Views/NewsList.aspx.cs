using System;
using NewsContainer=N2.Templates.Items.NewsContainer;

namespace N2.Templates.UI.Views
{
    public partial class NewsContainer : Web.UI.TemplatePage<Templates.Items.NewsContainer>
    {
        protected N2.Web.UI.WebControls.ItemDataSource idsNews;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            idsNews.Filtering += new EventHandler<N2.Collections.ItemListEventArgs>(idsNews_Filtering);
        }

        void idsNews_Filtering(object sender, N2.Collections.ItemListEventArgs e)
        {
            Collections.TypeFilter.Filter(e.Items, typeof(N2.Templates.Items.News));
        }
    }
}
