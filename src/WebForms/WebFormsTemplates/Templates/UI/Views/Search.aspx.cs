using System;
using System.Linq;
using System.Collections.Generic;
using N2.Persistence;

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
			Hits = Engine.Resolve<ITextSearcher>().Search(CurrentItem.SearchRoot, txtQuery.Text, 0, 100, out TotalCount)
				.Select(i => i.IsPage ? i : Find.ClosestPage(i))
				.Distinct()
				.Where(Filter.Is.AccessiblePage().Match).ToList();
			
            DataBind();
        }
    }
}