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
using System.IO;
using System.Web;
using System.Web.UI.WebControls;
using N2.Edit.Web;

namespace N2.Edit.FileManagement
{
	[ToolbarPlugIn("", "filemanager", "FileManagement/Default.aspx", ToolbarArea.Preview, "preview", "~/Edit/Img/Ico/folder.gif", 120, ToolTip = "file manager", GlobalResourceClassName = "Toolbar")]
	public partial class Default : UrlSelectionPage
	{
		#region Properties

		/// <summary>The currently selected url on this page.</summary>
		public string SelectedUrl
		{
			get { return selectedUrl.Value; }
			set { selectedUrl.Value = value; }
		}

		#endregion

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			string postBackUrl = Request.Form[selectedUrl.UniqueID];
			selectedUrl.Value = string.IsNullOrEmpty(postBackUrl) ? Request.QueryString["selectedUrl"] : postBackUrl;

			btnDelete.Attributes["onclick"] = string.Format(
				"return confirm('{0}' + document.getElementById('{1}').value);",
				"Confirm delete: ",
				selectedUrl.ClientID);
		}

		protected override void OnLoad(EventArgs e)
		{
			if (!IsPostBack)
			{
				hlItems.Visible = IsOpened && AllModesAvailable;
				hlItems.NavigateUrl = AppendQueryString("../ItemSelection/Default.aspx");
				if (!IsOpened)
					hlCancel.NavigateUrl = SelectedItem.Url;
			}

			base.OnLoad(e);
		}

		protected void fileView_TreeNodeDataBound(object sender, TreeNodeEventArgs e)
		{
			FileSiteMapNode fileNode = (FileSiteMapNode) e.Node.DataItem;
			if (fileNode.IsDirectory)
			{
				e.Node.ImageUrl = "../img/ico/folder.gif";
				e.Node.Target = "folder";
			}
			else
			{
				e.Node.ImageUrl = "../img/ico/page_white.gif";
				e.Node.Target = "file";
			}
			if (IsSelected(e.Node.NavigateUrl))
				e.Node.Select();
		}

		private string lastUrl = null;
		private bool IsSelected(string navigateUrl)
		{
			string url = Utility.ToAbsolute(navigateUrl);
			return (lastUrl != null) ? url == lastUrl : url == OpenerInputUrl;
		}

		protected void OnUploadClick(object sender, EventArgs e)
		{
			if (fileUpload.HasFile)
			{
				foreach (string key in Request.Files)
				{
					HttpPostedFile file = Request.Files[key];
					if (!string.IsNullOrEmpty(file.FileName))
					{
						string url = SelectedUrl + "/" + Path.GetFileName(file.FileName);

						if (!FileManager.CancelUploading(url))
						{
							lastUrl = url;
							string path = Server.MapPath(url);
							file.SaveAs(path);
							FileManager.InvokeUploaded(url);
						}
					}
				}
				fileView.DataBind();
			}
		}

		protected void OnCreateFolderClick(object sender, EventArgs e)
		{
			string url = SelectedUrl + "/" + txtFolder.Text;
			lastUrl = url;
			string path = Server.MapPath(url);
			Directory.CreateDirectory(path);

			fileView.DataBind();
			SelectedUrl = string.Empty;
		}

		private bool deleting = false;

		protected void OnDeleteCommand(object sender, CommandEventArgs args)
		{
			deleting = true;
			Validate();
			if (IsValid)
			{
				string path = Server.MapPath(SelectedUrl);

				if (!FileManager.CancelDeleting(selectedUrl.Value))
				{
					if (File.Exists(path))
						File.Delete(path);
					else if (Directory.Exists(path))
						Directory.Delete(path, true);
					FileManager.InvokeDeleted(selectedUrl.Value);

					lastUrl = SelectedUrl.Substring(0, SelectedUrl.TrimEnd('/').LastIndexOf('/'));
					
					DataBind();
				}
				SelectedUrl = string.Empty;
			}
		}

		protected void OnDeleteValidation(object source, ServerValidateEventArgs args)
		{
			if (deleting)
			{
				string uploadFolder = N2.Context.Current.EditManager.GetUploadFolderUrl();
				args.IsValid = !string.IsNullOrEmpty(SelectedUrl)
				               && !uploadFolder.Equals(SelectedUrl, StringComparison.InvariantCultureIgnoreCase);
			}
		}
	}
}