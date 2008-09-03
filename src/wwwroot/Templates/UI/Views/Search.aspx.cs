using System;
using System.Collections.Generic;

namespace N2.Templates.UI.Views
{
    public partial class Search : Web.UI.TemplatePage<N2.Templates.Items.AbstractSearch>
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Resources.Register.StyleSheet(this, "~/Search/UI/Css/Search.css", N2.Resources.Media.All);
        }

        private ICollection<ContentItem> hits = new List<ContentItem>();

        protected ICollection<ContentItem> Hits
        {
            get { return hits; }
            set { hits = value; }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            Hits = CurrentPage.Search(txtQuery.Text);
			
            DataBind();
        }
    }
}