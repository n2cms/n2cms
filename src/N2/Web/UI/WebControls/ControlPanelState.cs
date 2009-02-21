namespace N2.Web.UI.WebControls
{
	/// <summary>
	/// Display state for the control panel.
	/// </summary>
	public enum ControlPanelState
	{
		/// <summary>Who knows.</summary>
		Unknown = 0,
		/// <summary>The control panel is hidden (e.g. the user is not an editor).</summary>
		Hidden = 1,
		/// <summary>The control panel is visible displaying a page normally.</summary>
		Visible = 2,
		/// <summary>The control panel is displayed while editing the page in place.</summary>
		Editing = 4,
		/// <summary>The control panel is displayed during drag and drop of parts.</summary>
		DragDrop = 8,
		/// <summary>The control panel is displayed whlie previewing an item version.</summary>
		Previewing = 16
	}
}
