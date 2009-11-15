using System;

namespace N2.Edit.Navigation
{
	[ToolbarPlugin("", "tree", "navigation/tree.aspx?selected={selected}", ToolbarArea.Navigation, "navigation", "~/Edit/Img/Ico/sitemap_color.gif", -30, 
        ToolTip = "hierarchical navigation", 
        GlobalResourceClassName = "Toolbar", 
        SortOrder = -1)]
	public partial class Tree : NavigationPage
	{
		protected override void OnInit(EventArgs e)
		{
			siteTreeView.RootNode = RootNode;
            siteTreeView.SelectedItem = Selection.SelectedItem;
			siteTreeView.DataBind();

			Response.ExpiresAbsolute = DateTime.Now.AddDays(-1);

			base.OnInit(e);
		}
	}
}
