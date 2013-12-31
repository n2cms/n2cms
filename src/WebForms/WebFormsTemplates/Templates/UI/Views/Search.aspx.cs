using System;
using System.Linq;
using System.Collections.Generic;
using N2.Persistence.Search;
using System.Web.Security;
using N2.Definitions;

namespace N2.Templates.UI.Views
{
    public partial class Search : Web.UI.TemplatePage<N2.Templates.Items.AbstractSearch>
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Resources.Register.StyleSheet(this, "~/Templates/UI/Css/Search.css", N2.Resources.Media.All);
        }

        private IEnumerable<ContentItem> hits = new List<ContentItem>();

        protected IEnumerable<ContentItem> Hits
        {
            get { return hits; }
            set { hits = value; }
        }

        protected int TotalCount;

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            var query = Query.For(txtQuery.Text)
                .Below(CurrentItem.SearchRoot ?? Content.Traverse.StartPage)
                .Range(0, 100)
                .Pages(true)
                .State(ContentState.Published)
                .ReadableBy(User, Roles.GetRolesForUser)
                .Except(Query.For(typeof(ISystemNode)));
            var result = Engine.Resolve<IContentSearcher>().Search(query);
            Hits = result.Hits.Select(h => h.Content).Where(Content.Is.Accessible()).ToList();
            TotalCount = result.Total;
            
            DataBind();
        }
    }
}
