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
using N2.Definitions;
using N2.Edit.Web;
using N2.Integrity;

namespace N2.Edit
{
	[NavigationPlugin("Cut", "move", "javascript:n2nav.memorize('{selected}','move');", "", "~/edit/img/ico/cut.gif", 40, GlobalResourceClassName="Navigation")]
	[ToolbarPlugin("", "move", "javascript:n2.memorize('{selected}','move');", ToolbarArea.Navigation, "", "~/Edit/Img/Ico/cut.gif", 30, ToolTip = "move", GlobalResourceClassName = "Toolbar")]
	public partial class Move : EditPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				try
				{
					pnlNewName.Visible = false;
					ContentItem toMove = MemorizedItem;
					Engine.Persister.Move(toMove, SelectedItem);
					Refresh(toMove, ToolbarArea.Both);
				}
				catch (NameOccupiedException ex)
				{
					SetErrorMessage(cvMove, ex);
					pnlNewName.Visible = true;
				}
				catch (DestinationOnOrBelowItselfException ex)
				{
					SetErrorMessage(cvMove, ex);
				}
				catch (NotAllowedParentException ex)
				{
					SetErrorMessage(cvMove, ex);
				}
				catch (N2Exception ex)
				{
					SetErrorMessage(cvMove, ex);
				}

				LoadDefaultsAndInfo();
			}
		}

		private void LoadDefaultsAndInfo()
		{
			btnCancel.NavigateUrl = SelectedItem.RewrittenUrl;
			txtNewName.Text = MemorizedItem.Name;

			Title = string.Format(GetLocalResourceString("MovePage.TitleFormat"),
			                      MemorizedItem.Title,
			                      SelectedItem.Title);

			from.Text = string.Format(GetLocalResourceString("from.TextFormat"),
			                          GetBreadcrumbPath(MemorizedItem.Parent),
			                          MemorizedItem.Name);

			to.Text = string.Format(GetLocalResourceString("to.TextFormat"),
			                        GetBreadcrumbPath(SelectedItem),
			                        MemorizedItem.Name);

			itemsToMove.CurrentData = MemorizedItem;
			itemsToMove.DataBind();
		}

		protected void OnMoveClick(object sender, EventArgs e)
		{
			try
			{
				MemorizedItem.Name = txtNewName.Text;
				Engine.Persister.Move(MemorizedItem, SelectedItem);
				Refresh(MemorizedItem, ToolbarArea.Both);
			}
			catch (NameOccupiedException ex)
			{
				SetErrorMessage(cvMove, ex);
				pnlNewName.Visible = true;
			}
			catch (DestinationOnOrBelowItselfException ex)
			{
				SetErrorMessage(cvMove, ex);
			}
			catch (NotAllowedParentException ex)
			{
				SetErrorMessage(cvMove, ex);
			}
			catch (N2Exception ex)
			{
				SetErrorMessage(cvMove, ex);
			}
		}
	}
}