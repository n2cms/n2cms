using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Web;

namespace N2.Edit.Navigation
{
	[ToolbarPlugin("", "tree", "navigation/tree.aspx?selected={selected}", ToolbarArea.Navigation, "navigation", "~/Edit/Img/Ico/sitemap_color.gif", -30, ToolTip = "hierarchical navigation", GlobalResourceClassName = "Toolbar")]
	[SortPlugin]
	public partial class Tree : NavigationPage
	{
		protected override void OnInit(EventArgs e)
		{
			siteTreeView.RootNode = RootNode;
			siteTreeView.SelectedItem = SelectedItem;
			siteTreeView.DataBind();
	
			base.OnInit(e);
		}
	}
}
