using System;
using N2.Web;

namespace N2.Edit
{
	[NavigationLinkPlugin("Paste", "paste", "Content/paste.aspx?selected={selected}&memory={memory}&action={action}", Targets.Preview, "|Management|/Resources/icons/page_paste.png", 60, GlobalResourceClassName = "Navigation")]
	[ToolbarPlugin("PASTE", "paste", "Content/Paste.aspx?selected={selected}&memory={memory}&action={action}", ToolbarArea.Operations, Targets.Preview, "|Management|/Resources/icons/page_paste.png", 50, ToolTip = "paste", GlobalResourceClassName = "Toolbar")] 
	public partial class paste : Web.EditPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			string selected = Request.QueryString[PathData.SelectedQueryKey];
			string memory = Request.QueryString["memory"];
			string action = Request.QueryString["action"];
			if (string.IsNullOrEmpty(memory) || string.IsNullOrEmpty(action))
			{
				Title = "You must select what to paste and click on the appropriate action first.";
			}
			else
			{
				string url = string.Format("|Management|/Content/{0}.aspx?selected={1}&memory={2}", action, Server.UrlEncode(selected), Server.UrlEncode(memory));
				Response.Redirect(url);
			}
		}
	}
}
