using System;

namespace N2.Edit
{
	[NavigationPlugin("Paste", "paste", "../paste.aspx?selected={selected}&memory={memory}&action={action}", "preview", "~/edit/img/ico/page_paste.gif", 60, GlobalResourceClassName="Navigation")]
	[ToolbarPlugin("", "paste", "paste.aspx?selected={selected}&memory={memory}&action={action}", ToolbarArea.Navigation, "preview", "~/Edit/Img/Ico/page_paste.gif", 50, ToolTip = "paste", GlobalResourceClassName = "Toolbar")] 
	public partial class paste : Web.EditPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			string selected = Request.QueryString["selected"];
			string memory = Request.QueryString["memory"];
			string action = Request.QueryString["action"];
			if (string.IsNullOrEmpty(memory) || string.IsNullOrEmpty(action))
			{
				this.Title = "You must select what to paste and click on the appropriate action first.";
			}
			else
			{
				string url = string.Format("{0}.aspx?selected={1}&memory={2}", action, Server.UrlEncode(selected), Server.UrlEncode(memory));
				Response.Redirect(url);
			}
		}
	}
}
