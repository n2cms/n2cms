using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Edit;
using N2.Edit.Web;

namespace N2.Management.Files
{
	[ToolbarPlugin("FILES", "filemanager", "Files/Default.aspx?selected={selected}", ToolbarArea.Navigation, Targets.Top, "{ManagementUrl}/Resources/icons/folder.png", 120, 
		ToolTip = "file manager", 
		GlobalResourceClassName = "Toolbar")]
	public partial class Default : EditPage
	{
	}
}
