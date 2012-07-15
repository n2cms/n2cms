using N2.Edit;
using N2.Edit.Web;

namespace N2.Management.Files
{
	[ToolbarPlugin("FILES", "filemanager", "{ManagementUrl}/Files/Default.aspx?{Selection.SelectedQueryKey}={selected}", ToolbarArea.Navigation, Targets.Top, "{ManagementUrl}/Resources/icons/folder.png", 120, 
		ToolTip = "file manager", 
		GlobalResourceClassName = "Toolbar")]
	public partial class Default : EditPage
	{
		protected override void OnInit(System.EventArgs e)
		{
			base.OnInit(e);

			N2.Resources.Register.JQueryUi(Page);
			N2.Resources.Register.JQueryPlugins(Page);
		}
	}
}
