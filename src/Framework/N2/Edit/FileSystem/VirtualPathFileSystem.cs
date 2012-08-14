using System.Collections.Generic;
using System.IO;
using System.Web.Hosting;

namespace N2.Edit.FileSystem
{
	/// <summary>
	/// Wraps the virtual path provider file system.
	/// </summary>
	public class VirtualPathFileSystem : IFileSystem
	{
		VirtualPathProvider pathProvider = HostingEnvironment.VirtualPathProvider;
		public virtual VirtualPathProvider PathProvider
		{
			get { return pathProvider; }
			set { pathProvider = value; }
		}

		/// <summary>Gets files in directory.</summary>
		/// <param name="parentVirtualPath">The path to the directory.</param>
		/// <returns>An enumerations of files in the directory.</returns>
		public virtual IEnumerable<FileData> GetFiles(string parentVirtualPath)
		{
			if (!PathProvider.DirectoryExists(parentVirtualPath))
				yield break;
			
			foreach (VirtualFile file in PathProvider.GetDirectory(parentVirtualPath).Files)
				yield return GetFile(file.VirtualPath);
		}

		/// <summary>Gets file data.</summary>
		/// <param name="virtualPath">The path to the file.</param>
		/// <returns>A file data object.</returns>
		public virtual FileData GetFile(string virtualPath)
		{
			VirtualFile file = PathProvider.GetFile(virtualPath);

			if (file == null)
				return null;

			FileData f = new FileData();
			f.Name = file.Name;
			f.VirtualPath = file.VirtualPath;

			FileInfo fi = GetFileInfo(file.VirtualPath);
			if(fi != null && fi.Exists)
			{
				f.Created = fi.CreationTime;
				f.Updated = fi.LastWriteTime;
				f.Length = fi.Length;
			}

			return f;
		}

		/// <summary>Gets directories of a directory.</summary>
		/// <param name="parentVirtualPath">The path to the directory whose child directories to get.</param>
		/// <returns>An enumeration of directories.</returns>
		public virtual IEnumerable<DirectoryData> GetDirectories(string parentVirtualPath)
		{
			if (!PathProvider.DirectoryExists(parentVirtualPath))
				yield break;

			foreach (VirtualDirectory dir in PathProvider.GetDirectory(parentVirtualPath).Directories)
				yield return GetDirectory(dir.VirtualPath);
		}

		/// <summary>Gets a directory data.</summary>
		/// <param name="virtualPath">The path of the directory to get.</param>
		/// <returns>A directory data object.</returns>
		public virtual DirectoryData GetDirectory(string virtualPath)
		{
			VirtualDirectory dir = PathProvider.GetDirectory(virtualPath);
			if (dir == null)
				return null;

			DirectoryData d = new DirectoryData();
			d.Name = dir.Name;
			d.VirtualPath = dir.VirtualPath;

			DirectoryInfo di = GetDirectoryInfo(virtualPath);
			if (di != null)
			{
				d.Created = di.CreationTime;
				d.Updated = di.LastWriteTime;
			}

			return d;
		}

		/// <summary>Checks if a file exists.</summary>
		/// <param name="virtualPath">The path of the file to check.</param>
		/// <returns>True if the file exists in the file system.</returns>
		public virtual bool FileExists(string virtualPath)
		{
			return PathProvider.FileExists(virtualPath);
		}

		/// <summary>Checks the existence of a directory.</summary>
		/// <param name="virtualPath">The directory to check.</param>
		/// <returns>True if the directory exists.</returns>
		public virtual bool DirectoryExists(string virtualPath)
		{
			return PathProvider.DirectoryExists(virtualPath);
		}

		/// <summary>Delets a file from the file system.</summary>
		/// <param name="virtualPath">The path of the file to delete.</param>
		public virtual void DeleteFile(string virtualPath)
		{
			string path = MapPath(virtualPath);

			File.Delete(path);

			if (FileDeleted != null)
				FileDeleted.Invoke(this, new FileEventArgs(virtualPath, null));
		}

		/// <summary>Deletes a directory including all files and sub-directories.</summary>
		/// <param name="virtualPath">The path of the directory to remove.</param>
		public virtual void DeleteDirectory(string virtualPath)
		{
			string path = MapPath(virtualPath);
			Directory.Delete(path, true);

			if (DirectoryDeleted != null)
				DirectoryDeleted.Invoke(this, new FileEventArgs(virtualPath, null));
		}

		/// <summary>Moves a file to a new location.</summary>
		/// <param name="fromVirtualPath">The file path where the file is located.</param>
		/// <param name="destinationVirtualPath">The path the file should assume after beeing moved.</param>
		public virtual void MoveFile(string fromVirtualPath, string destinationVirtualPath)
		{
			string fromPath = MapPath(fromVirtualPath);
			string toPath = MapPath(destinationVirtualPath);

			File.Move(fromPath, toPath);

			if (FileMoved != null)
				FileMoved.Invoke(this, new FileEventArgs(destinationVirtualPath, fromVirtualPath));
		}

		/// <summary>Moves a directory.</summary>
		/// <param name="fromVirtualPath">The original directory path.</param>
		/// <param name="destinationVirtualPath">The path the directory should assume after beeing moved.</param>
		public virtual void MoveDirectory(string fromVirtualPath, string destinationVirtualPath)
		{
			string fromPath = MapPath(fromVirtualPath);
			string toPath = MapPath(destinationVirtualPath);

			Directory.Move(fromPath, toPath);

			if (DirectoryMoved != null)
				DirectoryMoved.Invoke(this, new FileEventArgs(destinationVirtualPath, fromVirtualPath));
		}

		/// <summary>Copies a file.</summary>
		/// <param name="fromVirtualPath">The path where the file is located.</param>
		/// <param name="destinationVirtualPath">The path the copy should assume when.</param>
		public virtual void CopyFile(string fromVirtualPath, string destinationVirtualPath)
		{
			string fromPath = MapPath(fromVirtualPath);
			string toPath = MapPath(destinationVirtualPath);

			File.Copy(fromPath, toPath);

			if (FileCopied != null)
				FileCopied.Invoke(this, new FileEventArgs(destinationVirtualPath, fromVirtualPath));
		}

		/// <summary>Creates a directory.</summary>
		/// <param name="virtualPath">The directory path to create.</param>
		public virtual void CreateDirectory(string virtualPath)
		{
			string path = MapPath(virtualPath);

			Directory.CreateDirectory(path);
			
			if (DirectoryCreated != null)
				DirectoryCreated.Invoke(this, new FileEventArgs(virtualPath, null));
		}

		public virtual Stream OpenFile(string virtualPath, bool readOnly = false)
		{
			if(FileExists(virtualPath))
			{
				VirtualFile file = PathProvider.GetFile(virtualPath);
				return file.Open();
			}

			FileAccess access = readOnly ? FileAccess.Read : FileAccess.ReadWrite;
			FileShare share = readOnly ? FileShare.Read : FileShare.None;

			return File.Open(MapPath(virtualPath), FileMode.OpenOrCreate, access, share);
		}

		/// <summary>Creates or overwrites a file at the given path.</summary>
		/// <param name="virtualPath">The path of the file to create.</param>
		/// <param name="inputStream">An input stream of the file contents.</param>
		public virtual void WriteFile(string virtualPath, Stream inputStream)
		{
			string path = MapPath(virtualPath);
			if (!Directory.Exists(Path.GetDirectoryName(path)))
				Directory.CreateDirectory((Path.GetDirectoryName(path)));
			
			if (FileExists(virtualPath))
			{
				using(Stream fileStream = File.OpenWrite(path))
				{
					long length = TransferBetweenStreams(inputStream, fileStream);
					fileStream.SetLength(length);
				}
			}
			else
			{
				using (Stream fileStream = File.Create(path))
				{
					TransferBetweenStreams(inputStream, fileStream);
				}
			}

			if (FileWritten != null)
				FileWritten.Invoke(this, new FileEventArgs(virtualPath, null));
		}

		long TransferBetweenStreams(Stream inputStream, Stream outputStream)
		{
			long inputStreamLength = 0;
			byte[] buffer = new byte[32768];
			while (true)
			{
				int bytesRead = inputStream.Read(buffer, 0, buffer.Length);
				if (bytesRead <= 0)
					break;

				outputStream.Write(buffer, 0, bytesRead);
				inputStreamLength += bytesRead;
			}
			return inputStreamLength;
		}

		/// <summary>Read file contents to a stream.</summary>
		/// <param name="virtualPath">The path of the file to read.</param>
		/// <param name="outputStream">The stream to which the file contents should be written.</param>
		public virtual void ReadFileContents(string virtualPath, Stream outputStream)
		{
			using (Stream sourceFile = OpenFile(virtualPath))
			{
				byte[] buffer = new byte[32768];
				while (true)
				{
					int bytesRead = sourceFile.Read(buffer, 0, buffer.Length);
					if (bytesRead <= 0)
						break;

					outputStream.Write(buffer, 0, bytesRead);
				}
			}
		}


		protected virtual FileInfo GetFileInfo(string virtualPath)
		{
			string physicalPath = MapPath(virtualPath);
			if (!File.Exists(physicalPath)) return null;
			
			return new FileInfo(physicalPath);
		}

		protected virtual DirectoryInfo GetDirectoryInfo(string virtualDir)
		{
			string physicalPath = MapPath(virtualDir);
			if (!Directory.Exists(physicalPath)) return null;

			return new DirectoryInfo(physicalPath);
		}

		protected virtual string MapPath(string virtualPath)
		{
			return HostingEnvironment.MapPath(virtualPath);
		}

		#region IFileSystem Members


		public event System.EventHandler<FileEventArgs> FileWritten;

		public event System.EventHandler<FileEventArgs> FileCopied;

		public event System.EventHandler<FileEventArgs> FileMoved;

		public event System.EventHandler<FileEventArgs> FileDeleted;

		public event System.EventHandler<FileEventArgs> DirectoryCreated;

		public event System.EventHandler<FileEventArgs> DirectoryMoved;

		public event System.EventHandler<FileEventArgs> DirectoryDeleted;

		#endregion
	}
}
