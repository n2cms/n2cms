using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Edit;
using N2.Edit.Web;

namespace Management.N2.Files
{
	[ToolbarPlugin("FILES", "filemanager", "Files/Default.aspx", ToolbarArea.Navigation, Targets.Top, "~/N2/Resources/Img/Ico/png/folder.png", 120, 
		ToolTip = "file manager", 
		GlobalResourceClassName = "Toolbar")]
	public partial class Default : EditPage
	{
	}
}
