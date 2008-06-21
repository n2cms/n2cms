using System;

namespace N2.Edit.Settings
{
	[ToolbarPlugin("", "settings", "settings/default.aspx?selected={selected}", ToolbarArea.Navigation, "navigation", "~/Edit/Img/Ico/wrench.gif", -10, ToolTip = "settings", GlobalResourceClassName = "Toolbar")]
	public partial class Default : Web.EditPage
	{
		protected void btnSave_Click(object sender, EventArgs e)
		{
			this.se.Save();
		}
	}
}
