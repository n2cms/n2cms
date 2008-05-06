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
using System.Text.RegularExpressions;
using N2.Persistence.Finder;

namespace N2.Edit.Navigation
{
	[ToolbarPlugin("", "search", "navigation/search.aspx?selected={selected}", ToolbarArea.Navigation, "navigation", "~/Edit/Img/Ico/page_find.gif", -15, ToolTip = "search", GlobalResourceClassName = "Toolbar")]
	public partial class Search : NavigationPage
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.txtQuery.Focus();
		}

		protected void btnSerach_Click(object sender, ImageClickEventArgs e)
		{
			this.idsItems.Query = CreateQuery();
			this.dgrItems.DataBind();
		}

		private IQueryEnding CreateQuery()
		{
			string likeQuery = "%" + this.txtQuery.Text + "%";
			N2.Persistence.Finder.IQueryAction query = N2.Find.Items
				.Where.Name.Like(likeQuery)
				.Or.SavedBy.Like(likeQuery)
				.Or.Title.Like(likeQuery)
				.Or.Detail().Like(likeQuery);

			if (Regex.IsMatch(this.txtQuery.Text, @"^\d+$", RegexOptions.Compiled))
				query = query.Or.ID.Eq(int.Parse(this.txtQuery.Text));

			return query.Filters(Engine.EditManager.GetEditorFilter(Page.User));
		}
	}
}
