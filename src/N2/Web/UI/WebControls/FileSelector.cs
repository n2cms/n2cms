using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Web.UI.WebControls
{
	/// <summary>An input box that can be updated with the url to a file through a popup window.</summary>
	public class FileSelector : UrlSelector
	{
		public FileSelector()
		{
			this.CssClass = "fileSelector urlSelector";
			this.DefaultMode = UrlSelectorMode.Files;
			this.AvailableModes = UrlSelectorMode.Files;
		}
	}
}
