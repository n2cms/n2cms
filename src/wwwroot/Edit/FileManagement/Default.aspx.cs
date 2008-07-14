using System;
using System.IO;
using System.Web;
using System.Web.UI.WebControls;
using N2.Edit.Web;

namespace N2.Edit.FileManagement
{
	[ToolbarPlugin("", "filemanager", "FileManagement/Default.aspx", ToolbarArea.Preview, Targets.Preview, "~/Edit/Img/Ico/folder.gif", 120, ToolTip = "file manager", GlobalResourceClassName = "Toolbar")]
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
			smds.Provider = new Web.FileSiteMapProvider();

			string postBackUrl = Request.Form[selectedUrl.UniqueID];
			selectedUrl.Value = string.IsNullOrEmpty(postBackUrl) ? Request.QueryString["selectedUrl"] : postBackUrl;

			btnDelete.Attributes["onclick"] = string.Format(
				"return confirm('{0}' + document.getElementById('{1}').value);",
				"Confirm delete: ",
				selectedUrl.ClientID);

			base.OnInit(e);
		}

		protected override void OnLoad(EventArgs e)
		{
			if (!IsPostBack)
			{
				hlItems.Visible = IsOpened && AllModesAvailable;
				string itemsUrl = AppendQueryString("../ItemSelection/Default.aspx");
				hlItems.NavigateUrl = itemsUrl;
				if (AllModesAvailable && string.IsNullOrEmpty(Request.QueryString["redirect"]) && !string.IsNullOrEmpty(selectedUrl.Value) && !selectedUrl.Value.StartsWith(Engine.EditManager.GetUploadFolderUrl()))
					Response.Redirect(itemsUrl);

				if (!IsOpened)
                    hlCancel.NavigateUrl = CancelUrl();
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
                    try
                    {
                        if (File.Exists(path))
                            File.Delete(path);
                        else if (Directory.Exists(path))
                            Directory.Delete(path, true);
                        FileManager.InvokeDeleted(selectedUrl.Value);
                    }
                    catch (Exception ex)
                    {
                        Trace.Write(ex.ToString());
                        cvDeleteProblem.IsValid = false;
                    }

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