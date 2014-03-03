using N2.Configuration;
using N2.Edit.FileSystem;
using N2.Edit.Web;
using N2.Web.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace N2.Management.Files
{
    public partial class Rebuild : EditPage
    {
        protected Dictionary<string, Configuration.ImageSizeElement> ConfiguredSizes;
        private UploadedFilesResizer Resizer;
        private IFileSystem Fs;

        protected void Page_Load(object sender, EventArgs e)
        {
            ConfiguredSizes = Engine.Config.Sections.Management.Images.Sizes.AllElements.ToDictionary(s => s.Name);
            Resizer = Engine.Resolve<UploadedFilesResizer>();
            Fs = Engine.Resolve<IFileSystem>();
        }

		protected void btnUpdate_Command(object sender, CommandEventArgs e)
		{
			var preExistingFiles = Fs.GetFilesRecursive(Selection.SelectedItem.Url)
				.Where(f => Resizer.IsResizableImagePath(f.VirtualPath))
				.Where(f => !f.Name.Contains('_'))
				.ToList();

			var resizer = Engine.Resolve<UploadedFilesResizer>();
			var previouslyEnabled = resizer.Enabled;
			try
			{
				resizer.Enabled = false;

				BuildImageSizes(preExistingFiles, Request["add"]);
				BuildImageSizes(preExistingFiles, Request["modify"]);
				RemoveImageSizes(preExistingFiles, Request["remove"]);
			}
			finally
			{
				resizer.Enabled = previouslyEnabled;
			}
		}

        private void RemoveImageSizes(List<FileData> preExistingFiles, string commaSeparatedListOfSizes)
        {
            if (string.IsNullOrEmpty(commaSeparatedListOfSizes))
                return;

            foreach (var s in commaSeparatedListOfSizes.Split(','))
            {
                foreach (var file in preExistingFiles)
                {
                    var resizedPath = ImagesUtility.GetResizedPath(file.VirtualPath, s);
                    if (Fs.FileExists(resizedPath))
                    {
                        Response.Write("<div class='processed-image'>Removing " + file.Name + " - " + s + "</div>");
                        Fs.DeleteFile(resizedPath);
                    }
                }
            }
        }

        private void BuildImageSizes(List<FileData> preExistingFiles, string commaSeparatedListOfSizes)
        {
            if (string.IsNullOrEmpty(commaSeparatedListOfSizes))
                return;

			foreach (var s in commaSeparatedListOfSizes.Split(','))
			{
				ImageSizeElement size;
				if (!ConfiguredSizes.TryGetValue(s, out size))
					continue;
				if (size.Height == 0 && size.Width == 0)
					continue;

                foreach (var file in preExistingFiles)
                {
                    Response.Write("<div class='processed-image'>Creating " + file.VirtualPath + "</div>");

                    var originalPath = ImagesUtility.GetResizedPath(file.VirtualPath, "original");
                    var sourcePath = Fs.FileExists(originalPath)
                        ? originalPath
                        : file.VirtualPath;

                    Resizer.CreateSize(file.VirtualPath, Resizer.GetImageBytes(sourcePath), size);
                }
            }
        }
    }
}
