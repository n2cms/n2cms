using System;
using N2.Edit.Web;

namespace N2.Edit.ItemSelection
{
	public partial class Default : UrlSelectionPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				hlFiles.Visible = AllModesAvailable;
				hlFiles.NavigateUrl = AppendQueryString("../FileManagement/Default.aspx");
			}
			siteTreeView.SelectedItem = SelectedItem;
			siteTreeView.DataBind();

			foreach (ContentItem item in Find.EnumerateTree(Find.RootItem))
			{
				ClientScript.RegisterArrayDeclaration("linkArray", string.Format("{{key:\"{0}\", value:\"{1}\"}}",
					item.RewrittenUrl,
					item.Url.Replace("\"", "\\\"")));
			}
			
		}
	}
}