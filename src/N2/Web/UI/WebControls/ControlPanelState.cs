using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Web.UI.WebControls
{
	/// <summary>
	/// State for the control panel.
	/// </summary>
	public enum ControlPanelState
	{
		/// <summary>The control panel is hidden (the user is not an editor).</summary>
		Hidden,
		Visible,
		Editing,
		DragDrop,
		Previewing
	}
}
