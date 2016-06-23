using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using N2.Configuration;
using N2.Edit.FileSystem.Items;
using N2.Edit.Web;
using N2.Resources;
using N2.Web.Drawing;
using System.Web;

namespace N2.Edit.FileSystem
{
    public partial class File1 : EditPage
    {
        protected IEnumerable<ContentItem> Ancestors;

		private IFileSystem Fs;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Page.StyleSheet("{ManagementUrl}/Files/Css/Files.css");

			Fs = Engine.Resolve<IFileSystem>();

            if (Selection == null || Selection.SelectedItem == null)
            {
                Ancestors = new ContentItem[0];
                btnEdit.Visible = false;
                hlCrop.Visible = false;
            }
            else
            {
                var config = Engine.Resolve<EditSection>();

                if (SelectedFile != null)
                {
                    if (config.FileSystem.IsTextFile(SelectedFile.Url))
                        btnEdit.Visible = true;

                    if (ImagesUtility.IsImagePath(Selection.SelectedItem.Url))
                    {
                        var size = config.Images.GetImageSize(SelectedFile.Url);
                        if (size != null && size.Mode == ImageResizeMode.Fill)
                            hlCrop.NavigateUrl = "../Crop.aspx?selected=" + Selection.SelectedItem.Path;
                        else
                            hlCrop.Visible = false;
                    }
                }

                Ancestors = Find.EnumerateParents(Selection.SelectedItem, null, true).Where(a => a is AbstractNode).Reverse();
                DataBind();
                LoadSizes();
                Refresh(Selection.SelectedItem, ToolbarArea.Navigation, force: false);
            }
        }

        private void LoadSizes()
        {
			if (Selection.SelectedItem == null)
				throw new ArgumentNullException("Selection.SelectedItem");

			var imageConfig = Engine.Config.Sections.Management.Images;
			if (imageConfig == null)
				throw new ArgumentNullException("imageConfig");
			else if (imageConfig.Sizes == null)
				throw new ArgumentNullException("imageConfig.Sizes");

			string baseImagePath, imageSize;
            ImagesUtility.SplitImageAndSize(Selection.SelectedItem.Url, imageConfig.Sizes.GetSizeNames(), out baseImagePath, out imageSize);

			if (string.IsNullOrEmpty(baseImagePath))
				throw new ArgumentException("baseImagePath");

			foreach (var size in imageConfig.Sizes.AllElements.Where(s => s.Announced))
				AddSize(baseImagePath, size);
        }

		private void AddSize(string baseImagePath, ImageSizeElement size)
		{
			if (size == null)
				throw new ArgumentNullException("size");

			bool exists;
			var path = Fs.GetExistingImagePath(baseImagePath, size.Name, out exists);
			if (exists)
			{
				if (SelectedFile == null)
					throw new ArgumentNullException("SelectedFile");

				var file = (SelectedFile.Parent as File) ?? SelectedFile;
				var child = file.GetChild(VirtualPathUtility.GetFileName(path));

				AddSizeControl(size, path, file, child);
			}
		}

		private void AddSizeControl(ImageSizeElement size, string path, File file, ContentItem child)
		{
			if (path == null)
				throw new ArgumentNullException("path");
			if (file == null)
				throw new ArgumentNullException("file");
			if (child == null)
				throw new ArgumentNullException("child");

			var hl = new HyperLink { ID = size.Name + "Size" };
			hl.NavigateUrl = "File.aspx?selected=" + child.Path;
			hl.Text = Utility.GetResourceString("ImageSizes", size.Name + ".Text") ?? (string.IsNullOrEmpty(size.Description) ? size.Name : size.Description);
			hl.Text += GetSizeText(size.Width, size.Height);
			hl.CssClass = "command";
			if (bgSizes == null)
				throw new ArgumentNullException("bgSizes");

			if (path == Selection.SelectedItem.Url)
				bgSizes.Controls.AddAt(0, hl);
			else
				bgSizes.Controls.Add(hl);
		}

        private static string GetSizeText(int w, int h)
        {
            if (w == 0 && h == 0)
                return " &infin;";
            if (w == 0)
                return string.Format(" &infin;x{0}", w);
            if (h == 0)
                return string.Format(" {0}x&infin;", w);
            return string.Format(" {0}x{1}", w, h);
        }

        protected override void RegisterToolbarSelection()
        {
            var script = GetToolbarSelectScript("preview");
            Register.JavaScript(this, script, ScriptPosition.Bottom, ScriptOptions.ScriptTags);
        }

        protected File SelectedFile
        {
            get { return Selection.SelectedItem as File; }
        }

        protected void OnDownloadCommand(object sender, CommandEventArgs e)
        {
            Response.ContentType = "application/octet-stream";
            Response.AppendHeader("Content-disposition", "attachment; filename=" + SelectedFile.Name);
            SelectedFile.TransmitTo(Response.OutputStream);
            Response.End();
        }

        protected void OnEditCommand(object sender, CommandEventArgs e)
        {
            txtContent.Text = SelectedFile.ReadFile();
            btnEdit.Visible = false;
            btnDownload.Visible = false;
            txtContent.Visible = true;
            btnSave.Visible = true;
            btnCancel.Visible = true;
        }

        protected void OnSaveCommand(object sender, CommandEventArgs e)
        {
            SelectedFile.WriteFile(txtContent.Text);
            OnCancelCommand(sender, e);
        }

        protected void OnCancelCommand(object sender, CommandEventArgs e)
        {
            btnEdit.Visible = true;
            btnDownload.Visible = true;
            txtContent.Visible = false;
            btnSave.Visible = false;
            btnCancel.Visible = false;
        }

        const double GiB = 1024 * 1024 * 1024;
        const double MiB = 1024 * 1024;
        const double KiB = 1024;

        protected string GetFileSize(long size)
        {
            if (size > GiB) return string.Format("{0:0.0} GB", size / GiB);
            if (size > MiB) return string.Format("{0:0.0} MB", size / MiB);
            if (size > KiB) return string.Format("{0:0.0} KB", size / KiB);
            return string.Format("{0} B", size);
        }
    }
}
