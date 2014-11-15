using System;
using N2.Collections;

namespace N2.Templates.UI.Layouts
{
    public partial class SubMenu : Web.UI.TemplateUserControl<ContentItem>
    {
        public ContentItem StartPage
        {
            get { return m.StartPage; }
            set { m.StartPage = value; }
        }

        public int StartLevel
        {
            get { return m.StartLevel; }
            set { m.StartLevel = value; }
        }

        protected override void OnInit(EventArgs e)
        {
            Page.InitComplete += InitMenu;

            base.OnInit(e);
        }

        private void InitMenu(object sender, EventArgs e)
        {
            ContentItem branchRoot = Find.AncestorAtLevel(StartLevel, Find.EnumerateParents(CurrentPage, StartPage), CurrentPage);

            if (branchRoot != null && branchRoot.GetChildPagesUnfiltered().Where(new NavigationFilter()).Count > 0)
                hsm.Text = N2.Web.Link.To(branchRoot).ToString();
            else
                this.Visible = false;
        }
    }
}
