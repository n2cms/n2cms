using System;
using System.Web;
using System.Web.UI.WebControls;
using N2.Edit.FileSystem;
using N2.Edit.Web;
using N2.Web;

namespace N2.Edit.FileManagement
{
    [ToolbarPlugin("FILES", "filemanager", "FileManagement/Default.aspx", ToolbarArea.Navigation, Targets.Preview, "~/Edit/Img/Ico/png/folder.png", 120, ToolTip = "file manager", GlobalResourceClassName = "Toolbar")]
	public partial class Default : UrlSelectionPage
	{
		#region Properties

		/// <summary>The currently selected url on this page.</summary>
		public string SelectedUrl
		{
			get { return selectedUrl.Value; }
			set { selectedUrl.Value = value; }
		}
		protected IFileSystem FileSystem
		{
			get { return Engine.Resolve<IFileSystem>(); }
		}

		#endregion

		protected override void OnInit(EventArgs e)
		{
			smds.Provider = new Web.FileSiteMapProvider();

			string postBackUrl = Request.Form[selectedUrl.UniqueID];
			selectedUrl.Value = string.IsNullOrEmpty(postBackUrl) ? Request.QueryString["selectedUrl"] : postBackUrl;
            btnDelete.OnClientClick = string.Format(
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
				if (AllModesAvailable && string.IsNullOrEmpty(Request.QueryString["redirect"]) && !string.IsNullOrEmpty(selectedUrl.Value) && !IsUploadFolder())
					Response.Redirect(itemsUrl);

				if (!IsOpened)
                    hlCancel.NavigateUrl = CancelUrl();
			}

			base.OnLoad(e);
		}

	    private bool IsUploadFolder()
	    {
	        foreach (string folder in Engine.EditManager.UploadFolders)
	        {
	            if (selectedUrl.Value.StartsWith(Url.ToAbsolute(folder)))
	                return true;
	        }
	        return false;
	    }

	    protected void fileView_TreeNodeDataBound(object sender, TreeNodeEventArgs e)
		{
			FileSiteMapNode fileNode = (FileSiteMapNode) e.Node.DataItem;
	        e.Node.ImageUrl = fileNode.IconUrl;
	        e.Node.Target = fileNode.Target;
			
			if (IsSelected(e.Node.NavigateUrl))
				e.Node.Select();
			else if(e.Node.Depth < 1)
				e.Node.Select();
		}

		private string lastUrl = null;
		private bool IsSelected(string navigateUrl)
		{
			string openerUrl = OpenerInputUrl;
			if(string.IsNullOrEmpty(openerUrl))
				return false;

			string url = Url.ToAbsolute(navigateUrl);
			if (lastUrl != null) 
				return url == lastUrl;

			return url == Url.ToAbsolute(openerUrl);
		}

		protected void OnUploadClick(object sender, EventArgs e)
		{
			if (fileUpload.HasFile)
			{
				foreach (string key in Request.Files)
				{
					HttpPostedFile postedFile = Request.Files[key];
					if (!string.IsNullOrEmpty(postedFile.FileName))
					{
						string url = SelectedUrl + "/" + System.IO.Path.GetFileName(postedFile.FileName);

						try
						{
							lastUrl = url;
							FileSystem.WriteFile(url, postedFile.InputStream);
						}
						catch (Exception ex)
						{
							cvDeleteException.ErrorMessage = ex.Message;
							cvDeleteException.IsValid = false;
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
			//string path = HostingEnvironment.MapPath(url);
			//Directory.CreateDirectory(path);
			FileSystem.CreateDirectory(url);

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
				string url = HttpUtility.UrlDecode(SelectedUrl);

				try
                {
					if(FileSystem.FileExists(url))
						FileSystem.DeleteFile(url);
					else if(FileSystem.DirectoryExists(url))
						FileSystem.DeleteDirectory(url);

					lastUrl = SelectedUrl.Substring(0, SelectedUrl.TrimEnd('/').LastIndexOf('/'));
					
					DataBind();
				}
				catch(ApplicationException ex)
				{
					cvDeleteException.ErrorMessage = ex.Message;
					cvDeleteException.IsValid = false;
				}
				catch (Exception ex)
				{
					Trace.Write(ex.ToString());
					cvDeleteProblem.IsValid = false;
				}

				SelectedUrl = string.Empty;
			}
		}

		protected void OnDeleteValidation(object source, ServerValidateEventArgs args)
		{
			if (deleting)
			{
                foreach(string folder in Engine.EditManager.UploadFolders)
                {
                    string uploadFolder = Url.ToAbsolute(folder);
                    if(string.IsNullOrEmpty(SelectedUrl) || SelectedUrl.Equals(uploadFolder, StringComparison.InvariantCultureIgnoreCase))
                    {
                        args.IsValid = false;
                        return;
                    }
                }
			}
		}
	}
}