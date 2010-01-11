namespace N2.Web.UI.WebControls
{
	/// <summary>
	/// An input box that can be updated with a page selected through a popup 
	/// window.
	/// </summary>
	public class ItemSelector : UrlSelector
	{
		public ItemSelector()
		{
			CssClass = "itemSelector urlSelector";
			DefaultMode = UrlSelectorMode.Items;
			AvailableModes = UrlSelectorMode.Items;
			BrowserUrl = N2.Web.Url.ToAbsolute("~/N2/Content/ItemSelection/Default.aspx");
		}

		/// <summary>Gets the selected item or null if none is selected.</summary>
		public ContentItem SelectedItem
		{
			get { return string.IsNullOrEmpty(Url) ? null : N2.Context.UrlParser.Parse(Url); }
			set { Url = value != null ? value.Url : ""; }
		}

		/// <summary>Gets the ID of the selected item or 0 if no item is selected.</summary>
		public int SelectedItemID
		{
			get { return SelectedItem != null ? SelectedItem.ID : 0; }
			set { SelectedItem = N2.Context.Persister.Get(value); }
		}
	}
}