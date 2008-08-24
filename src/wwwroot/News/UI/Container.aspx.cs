using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace N2.Templates.News.UI
{
	public partial class Container : Web.UI.TemplatePage<Items.NewsContainer>
	{
		protected N2.Web.UI.WebControls.ItemDataSource idsNews;

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			idsNews.Filtering += new EventHandler<N2.Collections.ItemListEventArgs>(idsNews_Filtering);
		}

		void idsNews_Filtering(object sender, N2.Collections.ItemListEventArgs e)
		{
			Collections.TypeFilter.Filter(e.Items, typeof(Items.News));
		}
	}
}
