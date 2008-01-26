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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace N2.Edit.Navigation
{
	[ToolbarPlugIn("", "table", "navigation/table.aspx?selected={selected}", ToolbarArea.Navigation, "navigation", "~/Edit/Img/Ico/table.gif", -20,
	   ToolTip = "tabular navigation",
	   GlobalResourceClassName = "Toolbar")]
	public partial class Table : NavigationPage, N2.Web.UI.IItemContainer
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			idsItems.Filtering += new EventHandler<N2.Collections.ItemListEventArgs>(FilterByIsPage);
			idsItems.Selected += new EventHandler<N2.Collections.ItemListEventArgs>(FilterByIsPage);
		}

		void FilterByIsPage(object sender, N2.Collections.ItemListEventArgs e)
		{
			// filter non-page items
			if (!PersonalSettings.DisplayDataItems)

				for (int i = e.Items.Count - 1; i >= 0; i--)
					if (!e.Items[i].IsPage)
						e.Items.RemoveAt(i);
		}

		protected int FindReplacementItemIndex(int offset, int itemIndex, IList<ContentItem> siblings)
		{
			// Depending on settings not all items are displayed
			for (int i = itemIndex + offset; i >= 0 && i < siblings.Count; i += offset)
			{
				if (PersonalSettings.DisplayDataItems)
					return i;
				else if (siblings[i].IsPage)
					return i;
			}
			return -1;
		}

		protected void OnDataGridItemCommand(object source, DataGridCommandEventArgs e)
		{
			int itemID = (int)dgrItems.DataKeys[e.Item.ItemIndex];
			ContentItem item = N2.Context.Persister.Get(itemID);
			if (e.CommandName == "SortUp" || e.CommandName == "SortDown")
			{
				IList<ContentItem> siblings = item.Parent.Children;

				int itemIndex = siblings.IndexOf(item);
				int offset = e.CommandName == "SortUp" ? -1 : 1;
				int itemToSwitchWithIndex = FindReplacementItemIndex(offset, itemIndex, siblings);

				// Switch
				if (itemToSwitchWithIndex >= 0)
				{
					siblings[itemIndex] = siblings[itemToSwitchWithIndex];
					siblings[itemToSwitchWithIndex] = item;

					IEnumerable<ContentItem> changedItems = Utility.UpdateSortOrder(siblings);
					foreach(ContentItem unsavedItem in changedItems)
						N2.Context.Persister.Save(unsavedItem);
				}
			}
			dgrItems.DataBind();
		}

		#region IItemContainer Members

		public ContentItem CurrentItem
		{
			get { return SelectedItem; }
		}

		#endregion

	}
}
