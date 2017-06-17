using System.IO;
using System.Linq;
using System.Web;
using N2.Configuration;
using N2.Edit;
using N2.Edit.FileSystem;
using N2.Engine;
using N2.Plugin;
using N2.Web;
using N2.Web.Drawing;

namespace N2.Management.Files
{
    [Service]
    public class UploadedFilesResizer : IAutoStart
    {
        IFileSystem files;
        ImageResizer resizer;
        ImagesElement images;
        string[] sizeNames;

		public bool Enabled { get; set; }
		public UploadedFilesResizer(IFileSystem files, ImageResizer resizer, EditSection config)
		{
			this.files = files;
			this.resizer = resizer;
			this.images = config.Images;
			this.Enabled = config.Images.ResizeUploadedImages;
			sizeNames = config.Images.Sizes.AllElements.Select(s => s.Name).ToArray();
		}

		void files_FileWritten(object sender, FileEventArgs e)
		{
			if (!Enabled)
				return;

			Url virtualPath = e.VirtualPath;

            if (!IsResizableImagePath(virtualPath))
                return;

            if(images.Sizes.Count == 0)
                return;

            byte[] image = GetImageBytes(virtualPath);

            foreach (ImageSizeElement size in images.Sizes.AllElements)
            {
                CreateSize(virtualPath, image, size);
            }
        }

        public virtual void CreateSize(Url virtualPath, byte[] image, ImageSizeElement size)
        {
            if (!size.ResizeOnUpload)
                return;

            string resizedPath = ImagesUtility.GetResizedPath(virtualPath, size.Name);

			using (var sourceStream = new MemoryStream(image))
			{
				if (size.Width <= 0 && size.Height <= 0)
				{
					using (var destinationStream = files.OpenFile(resizedPath))
					{
						int b;
						while ((b = sourceStream.ReadByte()) != -1)
						{
							destinationStream.WriteByte((byte)b);
						}
					}
				}
				else
				{
					// Delete the image before writing.
					// Fixes a weird bug where overwriting the original file while it still exists
					//  leaves the resized image the with the exact same file size as the original even 
					//  though it should be smaller.
					if (files.FileExists(resizedPath))
					{
						files.DeleteFile(resizedPath);
					}

					try
					{
						using (var destinationStream = files.OpenFile(resizedPath))
						{
							resizer.Resize(sourceStream, new ImageResizeParameters(size.Width, size.Height, size.Mode) { Quality = size.Quality }, destinationStream);
						}
					}
					catch
					{
					}
				}
			}
		}

        public virtual byte[] GetImageBytes(string virtualPath)
        {
            byte[] image;
            using (var s = files.OpenFile(virtualPath))
            {
                image = new byte[s.Length];
                s.Read(image, 0, image.Length);
            }
            return image;
        }

		void files_FileCopied(object sender, FileEventArgs e)
		{
			if (!Enabled)
				return;

			if (IsResizedPath(e.VirtualPath))
				return;
			
			foreach (ImageSizeElement size in images.Sizes.AllElements)
			{
				Url sourceUrl = e.SourcePath;
				Url destinationUrl = e.VirtualPath;

                string sourcePath = ImagesUtility.GetResizedPath(sourceUrl, size.Name);

                if (!files.FileExists(sourcePath))
                    continue;

                string destinationPath = ImagesUtility.GetResizedPath(destinationUrl, size.Name);
                if(!files.FileExists(destinationPath))
                    files.CopyFile(sourcePath, destinationPath);
            }
        }

        private bool IsResizedPath(string path)
        {
            string extensionlessPath = Url.RemoveAnyExtension(path);
            foreach (var sizeName in sizeNames)
            {
                if (extensionlessPath.EndsWith("_" + sizeName))
                    return true;
            }
            return false;
        }

		void files_FileMoved(object sender, FileEventArgs e)
		{
			if (!Enabled)
				return;

			if (!IsResizableImagePath(e.VirtualPath))
				return;
			
			foreach (ImageSizeElement size in images.Sizes.AllElements)
			{
				string source = ImagesUtility.GetResizedPath(e.SourcePath, size.Name);
				if (files.FileExists(source))
				{
					string destination = ImagesUtility.GetResizedPath(e.VirtualPath, size.Name);
					if (!files.FileExists(destination))
						files.MoveFile(source, destination);
				}
			}
		}

		void files_FileDeleted(object sender, FileEventArgs e)
		{
			if (!Enabled)
				return;

			if (!IsResizableImagePath(e.VirtualPath))
				return;

            foreach (ImageSizeElement size in images.Sizes.AllElements)
            {
                string resizedPath = ImagesUtility.GetResizedPath(e.VirtualPath, size.Name);

                if (files.FileExists(resizedPath))
                    files.DeleteFile(resizedPath);
            }
        }

        public bool IsResizableImagePath(string imageUrl)
        {
            string fileExtension = VirtualPathUtility.GetExtension(Url.PathPart(imageUrl));
            return images.ResizedExtensions.Contains(fileExtension.ToLower());
        }

        #region IAutoStart Members

        public void Start()
        {
            if (!images.ResizeUploadedImages)
                return;

            files.FileWritten += files_FileWritten;
            files.FileMoved += files_FileMoved;
            files.FileDeleted += files_FileDeleted;
            files.FileCopied += files_FileCopied;
        }

        public void Stop()
        {
            if (!images.ResizeUploadedImages)
                return;

            files.FileWritten -= files_FileWritten;
            files.FileMoved -= files_FileMoved;
            files.FileDeleted -= files_FileDeleted;
            files.FileCopied -= files_FileCopied;
        }

        #endregion
    }
}
