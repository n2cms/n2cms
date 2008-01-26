#region License

/* Copyright (C) 2006 Cristian Libardo
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA.
 */

#endregion

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
			BrowserUrl = Utility.ToAbsolute("~/Edit/ItemSelection/Default.aspx");
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