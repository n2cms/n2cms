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
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace N2.Edit
{
	[NavigationPlugIn("Copy", "copy", "javascript:n2nav.memorize('{selected}','copy');", "", "~/edit/img/ico/page_copy.gif", 50)]
	[ToolbarPlugIn("", "copy", "javascript:n2.memorize('{selected}','copy');", ToolbarArea.Navigation, "", "~/edit/img/ico/page_copy.gif", 40, 
		ToolTip = "copy",
		GlobalResourceClassName = "Toolbar")]
    public partial class Copy : Web.EditPage
    {
		protected N2.ContentItem MemorizedItem;

        protected void Page_Load(object sender, EventArgs e)
        {
			MemorizedItem = N2.Context.UrlParser.Parse(Request.QueryString["memory"]);

			if (!IsPostBack)
			{
				try
				{
					N2.ContentItem newItem = Engine.Persister.Copy(MemorizedItem, SelectedItem);
					Refresh(newItem, ToolbarArea.Both);
				}
				catch (N2.Integrity.NameOccupiedException ex)
				{
					this.pnlNewName.Visible = true;
					SetErrorMessage(this.cvCopy, ex);
				}
				catch (N2.Definitions.NotAllowedParentException ex)
				{
					SetErrorMessage(this.cvCopy, ex);
				}
				catch (N2Exception ex)
				{
					SetErrorMessage(this.cvCopy, ex);
				}
				LoadDefaultsAndInfo();
			}
		}

		private void LoadDefaultsAndInfo()
		{
			btnCancel.NavigateUrl = SelectedItem.RewrittenUrl;
			txtNewName.Text = MemorizedItem.Name;

			this.Title = string.Format(GetLocalResourceString("CopyPage.TitleFormat"), 
				MemorizedItem.Title, 
				SelectedItem.Title);

			this.from.Text = string.Format(GetLocalResourceString("from.TextFormat"),
				GetBreadcrumbPath(MemorizedItem.Parent), 
				MemorizedItem.Name);

			this.to.Text = string.Format(GetLocalResourceString("to.TextFormat"),
				GetBreadcrumbPath(SelectedItem), 
				MemorizedItem.Name);

			itemsToCopy.CurrentData = MemorizedItem;
			itemsToCopy.DataBind();
		}

        protected void OnCopyClick(object sender, EventArgs e)
        {
			try
			{
				pnlNewName.Visible = false;
				N2.ContentItem newItem = MemorizedItem.Clone(true);
				newItem.Name = txtNewName.Text;
				newItem = Engine.Persister.Copy(newItem, SelectedItem);
				Refresh(newItem, ToolbarArea.Both);
			}
			catch (N2.Integrity.NameOccupiedException ex)
			{
				this.pnlNewName.Visible = true;
				SetErrorMessage(this.cvCopy, ex);
			}
			catch (N2.Definitions.NotAllowedParentException ex)
			{
				SetErrorMessage(this.cvCopy, ex);
			}
			catch (N2Exception ex)
			{
				SetErrorMessage(this.cvCopy, ex);
			}
        }
    }
}
