namespace N2.Web.UI.WebControls
{
	/// <summary>
	/// State for the control panel.
	/// </summary>
	public enum ControlPanelState
	{
		Unknown = 0,
		/// <summary>The control panel is hidden (e.g. the user is not an editor).</summary>
		Hidden = 1,
		Visible = 2,
		Editing = 4,
		DragDrop = 8,
		Previewing = 16
	}
}
