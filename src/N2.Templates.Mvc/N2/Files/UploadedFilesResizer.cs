using System;
using System.Collections.Generic;
using System.Web;
using N2.Engine;
using N2.Plugin;
using N2.Edit.FileSystem;
using N2.Edit;
using N2.Web;
using N2.Configuration;
using N2.Management.Files;
using System.IO;

namespace N2.Management.Files
{
	[Service]
	public class UploadedFilesResizer : IAutoStart
	{
		IFileSystem files;
		ImageResizer resizer;
		ImagesElement images;

		public UploadedFilesResizer(IFileSystem files, ImageResizer resizer, EditSection config)
		{
			this.files = files;
			this.resizer = resizer;
			this.images = config.Images;
		}

		void files_FileWritten(object sender, FileEventArgs e)
		{
			if (!IsResizableImagePath(e.VirtualPath))
				return;

			if(images.Sizes.Count == 0)
				return;

			byte[] image;
			using (var s = files.OpenFile(e.VirtualPath))
			{
				image = new byte[s.Length];
				s.Read(image, 0, image.Length);
			}

			foreach (ImageSizeElement size in images.Sizes.AllElements)
			{
				Url url = e.VirtualPath;
				string resizedPath = ImagesUtility.GetResizedPath(url, size.Name);

				using (var sourceStream = new MemoryStream(image))
				{
					if (size.Width <= 0 && size.Height <= 0)
					{
						using (var destinationStream = files.OpenFile(resizedPath))
						{
							int b;
							while((b = sourceStream.ReadByte()) != -1) 
							{
								destinationStream.WriteByte((byte)b);
							}
						}
					}
					else
					{
						if (!files.FileExists(resizedPath) || size.Replace)
						{
							using (var destinationStream = files.OpenFile(resizedPath))
							{
								resizer.Resize(sourceStream, url.Extension, size.Width, size.Height, size.Mode, destinationStream);
							}
						}
					}
				}
			}
		}

		void files_FileCopied(object sender, FileEventArgs e)
		{
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
			foreach (ImageSizeElement size in images.Sizes.AllElements)
			{
				if (Url.RemoveExtension(path).EndsWith("_" + size.Name))
					return true;
			}
			return false;
		}

		void files_FileMoved(object sender, FileEventArgs e)
		{
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