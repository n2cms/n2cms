using System;

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
			BrowserUrl = N2.Web.Url.Parse(Engine.ManagementPaths.EditTreeUrl).AppendQuery("location=filesselection");
		}
	}
}
