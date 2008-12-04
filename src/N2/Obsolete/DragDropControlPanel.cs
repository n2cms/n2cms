using System;
using System.Web.UI;

[assembly: WebResource("N2.Resources.layout_edit.png", "image/png")]
[assembly: WebResource("N2.Resources.page_refresh.png", "image/png")]

namespace N2.Web.UI.WebControls
{
	[Obsolete("This is now integrated in ControlPanel.")]
	public class DragDropControlPanel : ControlPanel
	{
	}
}