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
		public IEnumerable<FileData> GetFiles(string parentVirtualPath)
		{
			if (!HostingEnvironment.VirtualPathProvider.DirectoryExists(parentVirtualPath))
				yield break;
			
			foreach (VirtualFile file in HostingEnvironment.VirtualPathProvider.GetDirectory(parentVirtualPath).Files)
				yield return GetFile(file.VirtualPath);
		}

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

		public IEnumerable<DirectoryData> GetDirectories(string parentVirtualPath)
		{
			if (!HostingEnvironment.VirtualPathProvider.DirectoryExists(parentVirtualPath))
				yield break;

			foreach (VirtualDirectory dir in HostingEnvironment.VirtualPathProvider.GetDirectory(parentVirtualPath).Directories)
				yield return GetDirectory(dir.VirtualPath);
		}

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

		public bool FileExists(string virtualPath)
		{
			return HostingEnvironment.VirtualPathProvider.FileExists(virtualPath);
		}

		public bool DirectoryExists(string virtualPath)
		{
			return HostingEnvironment.VirtualPathProvider.DirectoryExists(virtualPath);
		}

		public void DeleteFile(string virtualPath)
		{
			string path = HostingEnvironment.MapPath(virtualPath);

			File.Delete(path);
		}

		public void DeleteDirectory(string virtualPath)
		{
			string path = HostingEnvironment.MapPath(virtualPath);
			Directory.Delete(path);
		}

		public void MoveFile(string fromVirtualPath, string destinationVirtualPath)
		{
			string fromPath = HostingEnvironment.MapPath(fromVirtualPath);
			string toPath = HostingEnvironment.MapPath(destinationVirtualPath);

			File.Move(fromPath, toPath);
		}

		public void MoveDirectory(string fromVirtualPath, string destinationVirtualPath)
		{
			string fromPath = HostingEnvironment.MapPath(fromVirtualPath);
			string toPath = HostingEnvironment.MapPath(destinationVirtualPath);

			Directory.Move(fromPath, toPath);
		}

		public void CopyFile(string fromVirtualPath, string destinationVirtualPath)
		{
			string fromPath = HostingEnvironment.MapPath(fromVirtualPath);
			string toPath = HostingEnvironment.MapPath(destinationVirtualPath);

			File.Copy(fromPath, toPath);
		}

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
