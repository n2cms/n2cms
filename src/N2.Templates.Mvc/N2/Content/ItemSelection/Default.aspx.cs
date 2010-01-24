using N2.Resources;
using System;
using N2.Edit.Web;
using N2.Web;

namespace N2.Edit.ItemSelection
{
	using Resources;
	
	public partial class Default : UrlSelectionPage
	{
		protected override void OnInit(EventArgs e)
		{
			//css to hilight selection..
			Register.StyleSheet(this.Page, @"~\N2\Files\Css\filemanager.css");
			
			//focus on selection
			Register.JavaScript(this.Page, "$('.selected').focus();", ScriptOptions.DocumentReady);
			
			base.OnInit(e);
		}
		
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				hlFiles.Visible = AllModesAvailable;
				hlFiles.NavigateUrl = AppendQueryString("../../Files/Selector.aspx");
			}
			siteTreeView.RootNode = RootNode;
            siteTreeView.SelectedItem = Selection.SelectedItem;
			siteTreeView.DataBind();

			foreach (ContentItem item in Find.EnumerateTree(Find.RootItem))
			{
				ClientScript.RegisterArrayDeclaration("linkArray", string.Format("{{key:\"{0}\", value:\"{1}\"}}",
					item.FindPath(PathData.DefaultAction).RewrittenUrl,
					item.Url.Replace("\"", "\\\"")));
			}
		}
	}
}