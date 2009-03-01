using System.Collections.Generic;
using System.Web.Hosting;
using System.IO;

namespace N2.Edit.FileSystem
{
	/// <summary>
	/// Wraps the virtual path provider file system.
	/// </summary>
	public class VirtualPathFileSystem : IFileSystem
	{
		/// <summary>Gets files in directory.</summary>
		/// <param name="parentVirtualPath">The path to the directory.</param>
		/// <returns>An enumerations of files in the directory.</returns>
		public IEnumerable<FileData> GetFiles(string parentVirtualPath)
		{
			if (!HostingEnvironment.VirtualPathProvider.DirectoryExists(parentVirtualPath))
				yield break;
			
			foreach (VirtualFile file in HostingEnvironment.VirtualPathProvider.GetDirectory(parentVirtualPath).Files)
				yield return GetFile(file.VirtualPath);
		}

		/// <summary>Gets file data.</summary>
		/// <param name="virtualPath">The path to the file.</param>
		/// <returns>A file data object.</returns>
		public FileData GetFile(string virtualPath)
		{
			VirtualFile file = HostingEnvironment.VirtualPathProvider.GetFile(virtualPath);

			FileData f = new FileData();
			f.Name = file.Name;
			f.VirtualPath = file.VirtualPath;

			string physicalPath = HostingEnvironment.MapPath(file.VirtualPath);
			if(File.Exists(physicalPath))
			{
				FileInfo fi = new FileInfo(physicalPath);
				f.Created = fi.CreationTime;
				f.Updated = fi.LastWriteTime;
				f.Length = fi.Length;
			}

			return f;
		}

		/// <summary>Gets directories of a directory.</summary>
		/// <param name="parentVirtualPath">The path to the directory whose child directories to get.</param>
		/// <returns>An enumeration of directories.</returns>
		public IEnumerable<DirectoryData> GetDirectories(string parentVirtualPath)
		{
			if (!HostingEnvironment.VirtualPathProvider.DirectoryExists(parentVirtualPath))
				yield break;

			foreach (VirtualDirectory dir in HostingEnvironment.VirtualPathProvider.GetDirectory(parentVirtualPath).Directories)
				yield return GetDirectory(dir.VirtualPath);
		}

		/// <summary>Gets a directory data.</summary>
		/// <param name="virtualPath">The path of the directory to get.</param>
		/// <returns>A directory data object.</returns>
		public DirectoryData GetDirectory(string virtualPath)
		{
			VirtualDirectory dir = HostingEnvironment.VirtualPathProvider.GetDirectory(virtualPath);

			DirectoryData d = new DirectoryData();
			d.Name = dir.Name;
			d.VirtualPath = dir.VirtualPath;

			string physicalPath = HostingEnvironment.MapPath(dir.VirtualPath);
			if (Directory.Exists(physicalPath))
			{
				DirectoryInfo di = new DirectoryInfo(physicalPath);
				d.Created = di.CreationTime;
				d.Updated = di.LastWriteTime;
			}

			return d;
		}

		/// <summary>Checks if a file exists.</summary>
		/// <param name="virtualPath">The path of the file to check.</param>
		/// <returns>True if the file exists in the file system.</returns>
		public bool FileExists(string virtualPath)
		{
			return HostingEnvironment.VirtualPathProvider.FileExists(virtualPath);
		}

		/// <summary>Checks the existence of a directory.</summary>
		/// <param name="virtualPath">The directory to check.</param>
		/// <returns>True if the directory exists.</returns>
		public bool DirectoryExists(string virtualPath)
		{
			return HostingEnvironment.VirtualPathProvider.DirectoryExists(virtualPath);
		}

		/// <summary>Delets a file from the file system.</summary>
		/// <param name="virtualPath">The path of the file to delete.</param>
		public void DeleteFile(string virtualPath)
		{
			string path = HostingEnvironment.MapPath(virtualPath);

			File.Delete(path);
		}

		/// <summary>Deletes a directory including all files and sub-directories.</summary>
		/// <param name="virtualPath">The path of the directory to remove.</param>
		public void DeleteDirectory(string virtualPath)
		{
			string path = HostingEnvironment.MapPath(virtualPath);
			Directory.Delete(path, true);
		}

		/// <summary>Moves a file to a new location.</summary>
		/// <param name="fromVirtualPath">The file path where the file is located.</param>
		/// <param name="destinationVirtualPath">The path the file should assume after beeing moved.</param>
		public void MoveFile(string fromVirtualPath, string destinationVirtualPath)
		{
			string fromPath = HostingEnvironment.MapPath(fromVirtualPath);
			string toPath = HostingEnvironment.MapPath(destinationVirtualPath);

			File.Move(fromPath, toPath);
		}

		/// <summary>Moves a directory.</summary>
		/// <param name="fromVirtualPath">The original directory path.</param>
		/// <param name="destinationVirtualPath">The path the directory should assume after beeing moved.</param>
		public void MoveDirectory(string fromVirtualPath, string destinationVirtualPath)
		{
			string fromPath = HostingEnvironment.MapPath(fromVirtualPath);
			string toPath = HostingEnvironment.MapPath(destinationVirtualPath);

			Directory.Move(fromPath, toPath);
		}

		/// <summary>Copies a file.</summary>
		/// <param name="fromVirtualPath">The path where the file is located.</param>
		/// <param name="destinationVirtualPath">The path the copy should assume when.</param>
		public void CopyFile(string fromVirtualPath, string destinationVirtualPath)
		{
			string fromPath = HostingEnvironment.MapPath(fromVirtualPath);
			string toPath = HostingEnvironment.MapPath(destinationVirtualPath);

			File.Copy(fromPath, toPath);
		}

		/// <summary>Creates a directory.</summary>
		/// <param name="virtualPath">The directory path to create.</param>
		public void CreateDirectory(string virtualPath)
		{
			string path = HostingEnvironment.MapPath(virtualPath);

			Directory.CreateDirectory(path);
		}

		public Stream OpenFile(string virtualPath)
		{
			if(FileExists(virtualPath))
			{
				VirtualFile file = HostingEnvironment.VirtualPathProvider.GetFile(virtualPath);
				return file.Open();
			}
			return File.Open(HostingEnvironment.MapPath(virtualPath), FileMode.OpenOrCreate);
		}

		/// <summary>Creates or overwrites a file at the given path.</summary>
		/// <param name="virtualPath">The path of the file to create.</param>
		/// <param name="inputStream">An input stream of the file contents.</param>
		public void WriteFile(string virtualPath, Stream inputStream)
		{
			using (Stream destinationFile = OpenFile(virtualPath))
			{
				byte[] buffer = new byte[32768];
				while (true)
				{
					int bytesRead = inputStream.Read(buffer, 0, buffer.Length);
					if (bytesRead <= 0)
						break;

					destinationFile.Write(buffer, 0, bytesRead);
				}
			}
		}

		/// <summary>Read file contents to a stream.</summary>
		/// <param name="virtualPath">The path of the file to read.</param>
		/// <param name="outputStream">The stream to which the file contents should be written.</param>
		public void ReadFileContents(string virtualPath, Stream outputStream)
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
	}
}
