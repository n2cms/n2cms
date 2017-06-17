using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Engine;
using N2.Persistence.Search;
using N2.Persistence;

namespace N2.Management.Search
{
    public partial class ManageIndex : System.Web.UI.UserControl
    {
        protected IEngine Engine { get; set; }
        protected IndexStatus Status { get; set; }
        protected IndexStatistics Statistics { get; set; }
		protected string IndexerType { get; set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            
            Engine = N2.Context.Current;
        }

        protected void OnClear(object sender, CommandEventArgs args)
        {
            Engine.Resolve<IContentIndexer>().Clear();
        }

        protected void OnIndex(object sender, CommandEventArgs args)
        {
            Engine.Resolve<IAsyncIndexer>().ReindexDescendants(Engine.Content.Traverse.RootPage.ID, false);
        }

        protected void OnReindex(object sender, CommandEventArgs args)
        {
            Engine.Resolve<IAsyncIndexer>().ReindexDescendants(Engine.Content.Traverse.RootPage.ID, true);
        }

        protected void OnSearch(object sender, CommandEventArgs args)
        {
            rptSearch.DataSource = Engine.Resolve<IContentSearcher>()
                .Search(Query.For(txtSearch.Text))
                .Hits.Where(h => h.Content.IsAuthorized(Page.User));
            rptSearch.DataBind();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

			IndexerType = Engine.Resolve<IIndexer>().GetType().Name;
			Status = Engine.Resolve<IAsyncIndexer>().GetCurrentStatus();
            Statistics = Engine.Resolve<IContentIndexer>().GetStatistics();
        }
	}
}
