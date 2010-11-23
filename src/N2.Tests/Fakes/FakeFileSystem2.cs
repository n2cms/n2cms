using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Edit.FileSystem;
using System.Web;

namespace N2.Tests.Fakes
{
	public class FakeFileSystem2 : IFileSystem
	{
		public Dictionary<string, DirectoryData> directories = new Dictionary<string,DirectoryData>();
		public Dictionary<string, FileData> files = new Dictionary<string,FileData>();

		public IEnumerable<FileData> GetFiles(string parentVirtualPath)
		{
			return files.Keys.Where(k => k.StartsWith(parentVirtualPath)).Select(k => files[k]);
		}

		public FileData GetFile(string virtualPath)
		{
			return files[virtualPath];
		}

		public IEnumerable<DirectoryData> GetDirectories(string parentVirtualPath)
		{
			return directories.Keys.Where(k => k.StartsWith(parentVirtualPath)).Select(k => directories[k]);
		}

		public DirectoryData GetDirectory(string virtualPath)
		{
			return directories[virtualPath];
		}

		public bool FileExists(string virtualPath)
		{
			return files.ContainsKey(virtualPath);
		}

		public void MoveFile(string fromVirtualPath, string destinationVirtualPath)
		{
			files[destinationVirtualPath] = files[fromVirtualPath];
			files.Remove(fromVirtualPath);
		}

		public void DeleteFile(string virtualPath)
		{
			files.Remove(virtualPath);
		}

		public void CopyFile(string fromVirtualPath, string destinationVirtualPath)
		{
			files[destinationVirtualPath] = files[fromVirtualPath];
		}

		public System.IO.Stream OpenFile(string virtualPath)
		{
			throw new NotImplementedException();
		}

		public void WriteFile(string virtualPath, System.IO.Stream inputStream)
		{
			throw new NotImplementedException();
		}

		public void ReadFileContents(string virtualPath, System.IO.Stream outputStream)
		{
			throw new NotImplementedException();
		}

		public bool DirectoryExists(string virtualPath)
		{
			return directories.ContainsKey(virtualPath);
		}

		public void MoveDirectory(string fromVirtualPath, string destinationVirtualPath)
		{
			directories[destinationVirtualPath] = directories[fromVirtualPath];
			directories.Remove(fromVirtualPath);
		}

		public void DeleteDirectory(string virtualPath)
		{
			directories.Remove(virtualPath);
		}

		public void CreateDirectory(string virtualPath)
		{
			directories[virtualPath] = new DirectoryData { VirtualPath = virtualPath, Created = DateTime.Now, Updated = DateTime.Now, Name = N2.Web.Url.GetName(virtualPath) };
		}

		public event EventHandler<N2.Edit.FileEventArgs> FileWritten = delegate { };

		public event EventHandler<N2.Edit.FileEventArgs> FileCopied = delegate { };

		public event EventHandler<N2.Edit.FileEventArgs> FileMoved = delegate { };

		public event EventHandler<N2.Edit.FileEventArgs> FileDeleted = delegate { };

		public event EventHandler<N2.Edit.FileEventArgs> DirectoryCreated = delegate { };

		public event EventHandler<N2.Edit.FileEventArgs> DirectoryMoved = delegate { };

		public event EventHandler<N2.Edit.FileEventArgs> DirectoryDeleted = delegate { };
	}
}
