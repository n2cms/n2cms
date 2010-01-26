using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Edit;
using N2.Edit.Web;

namespace Management.N2.Files
{
	[ToolbarPlugin("VIEW", "filesPreview", "{url}", ToolbarArea.Files, Targets.Preview, "~/N2/Resources/Img/Ico/Png/eye.png", 0, 
		ToolTip = "Preview", 
		GlobalResourceClassName = "Toolbar")]
	[ToolbarPlugin("FILES", "filemanager", "Files/Default.aspx?selected={selected}", ToolbarArea.Navigation, Targets.Top, "~/N2/Resources/Img/Ico/png/folder.png", 120, 
		ToolTip = "file manager", 
		GlobalResourceClassName = "Toolbar")]
	public partial class Default : EditPage
	{
	}
}
