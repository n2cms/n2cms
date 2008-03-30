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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Web;

namespace N2.Edit.Navigation
{
	[ToolbarPlugin("", "tree", "navigation/tree.aspx?selected={selected}", ToolbarArea.Navigation, "navigation", "~/Edit/Img/Ico/sitemap_color.gif", -30, ToolTip = "hierarchical navigation", GlobalResourceClassName = "Toolbar")]
	[SortPlugin]
	public partial class Tree : NavigationPage
	{
		protected override void OnInit(EventArgs e)
		{
			siteTreeView.RootNode = RootNode;
			siteTreeView.SelectedItem = SelectedItem;
			siteTreeView.DataBind();
	
			base.OnInit(e);
		}
	}
}
