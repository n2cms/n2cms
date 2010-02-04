using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace N2.Edit.FileSystem
{
	/// <summary>
	/// Uses HttpContext.Current and System.IO to operate files.
	/// </summary>
	public class MappedFileSystem : IFileSystem
	{
		#region IFileSystem Members

		public IEnumerable<FileData> GetFiles(string parentVirtualPath)
		{
			foreach (var file in new DirectoryInfo(MapPath(parentVirtualPath)).GetFiles())
				if(!file.Name.StartsWith("."))
					yield return GetFile(N2.Web.Url.Combine(parentVirtualPath, file.Name));
		}

		public FileData GetFile(string virtualPath)
		{
			FileInfo info = new FileInfo(MapPath(virtualPath));
			return new FileData
			{
				Name = info.Name,
				Created = info.CreationTime,
				Updated = info.LastWriteTime,
				VirtualPath = virtualPath,
				Length = info.Length
			};
		}

		public IEnumerable<DirectoryData> GetDirectories(string parentVirtualPath)
		{
			foreach (var dir in new DirectoryInfo(MapPath(parentVirtualPath)).GetDirectories())
				if (!dir.Name.StartsWith("."))
					yield return GetDirectory(N2.Web.Url.Combine(parentVirtualPath, dir.Name));
		}

		public DirectoryData GetDirectory(string virtualPath)
		{
			DirectoryInfo info = new DirectoryInfo(MapPath(virtualPath));
			return new DirectoryData 
			{ 
				Name = info.Name, 
				Created = info.CreationTime, 
				Updated = info.LastWriteTime, 
				VirtualPath = virtualPath 
			};
		}

		public bool FileExists(string virtualPath)
		{
			return File.Exists(MapPath(virtualPath));
		}

		public void MoveFile(string fromVirtualPath, string destinationVirtualPath)
		{
			File.Move(MapPath(fromVirtualPath), MapPath(destinationVirtualPath));

			if (FileMoved != null)
				FileMoved.Invoke(this, new FileEventArgs(destinationVirtualPath, fromVirtualPath));
		}

		public void DeleteFile(string virtualPath)
		{
			File.Delete(MapPath(virtualPath));

			if (FileDeleted != null)
				FileDeleted.Invoke(this, new FileEventArgs(virtualPath, null));
		}

		public void CopyFile(string fromVirtualPath, string destinationVirtualPath)
		{
			File.Copy(MapPath(fromVirtualPath), MapPath(destinationVirtualPath));

			if (FileCopied != null)
				FileCopied.Invoke(this, new FileEventArgs(destinationVirtualPath, fromVirtualPath));
		}

		public System.IO.Stream OpenFile(string virtualPath)
		{
			return File.Open(MapPath(virtualPath), FileMode.OpenOrCreate);
		}

		public void WriteFile(string virtualPath, System.IO.Stream inputStream)
		{
			if (FileExists(virtualPath))
			{
				using (var s = File.OpenWrite(MapPath(virtualPath)))
				{
					TransferBetweenStreams(inputStream, s);
				}
			}
			else
			{
				using (var s = File.Create(MapPath(virtualPath)))
				{
					TransferBetweenStreams(inputStream, s);
				}
			}

			if (FileWritten != null)
				FileWritten.Invoke(this, new FileEventArgs(virtualPath, null));
		}

		public void ReadFileContents(string virtualPath, System.IO.Stream outputStream)
		{
			TransferBetweenStreams(File.OpenRead(MapPath(virtualPath)), outputStream);
		}

		public bool DirectoryExists(string virtualPath)
		{
			return Directory.Exists(MapPath(virtualPath));
		}

		public void MoveDirectory(string fromVirtualPath, string destinationVirtualPath)
		{
			Directory.Move(MapPath(fromVirtualPath), MapPath(destinationVirtualPath));

			if (DirectoryMoved != null)
				DirectoryMoved.Invoke(this, new FileEventArgs(destinationVirtualPath, fromVirtualPath));
		}

		public void DeleteDirectory(string virtualPath)
		{
			Directory.Delete(MapPath(virtualPath), true);

			if (DirectoryDeleted != null)
				DirectoryDeleted.Invoke(this, new FileEventArgs(virtualPath, null));
		}

		public void CreateDirectory(string virtualPath)
		{
			Directory.CreateDirectory(MapPath(virtualPath));

			if (DirectoryCreated != null)
				DirectoryCreated.Invoke(this, new FileEventArgs(virtualPath, null));
		}

		public event EventHandler<FileEventArgs> FileWritten;

		public event EventHandler<FileEventArgs> FileCopied;

		public event EventHandler<FileEventArgs> FileMoved;

		public event EventHandler<FileEventArgs> FileDeleted;

		public event EventHandler<FileEventArgs> DirectoryCreated;

		public event EventHandler<FileEventArgs> DirectoryMoved;

		public event EventHandler<FileEventArgs> DirectoryDeleted;

		#endregion

		protected virtual void TransferBetweenStreams(Stream inputStream, Stream outputStream)
		{
			byte[] buffer = new byte[32768];
			while (true)
			{
				int bytesRead = inputStream.Read(buffer, 0, buffer.Length);
				if (bytesRead <= 0)
					break;

				outputStream.Write(buffer, 0, bytesRead);
			}
		}

		protected virtual string MapPath(string virtualPath)
		{
			return HttpContext.Current.Server.MapPath(virtualPath);
		}
	}
}
