using System.Collections.Generic;
using System.IO;

namespace N2.Edit.FileSystem
{
	/// <summary>
	/// Abstracts operations against binary files.
	/// </summary>
	public interface IFileSystem
	{
		IEnumerable<FileData> GetFiles(string parentVirtualPath); // gets files in directory
		FileData GetFile(string virtualPath);
		IEnumerable<DirectoryData> GetDirectories(string parentVirtualPath); // gets directories in directory
		DirectoryData GetDirectory(string virtualPath);

		bool FileExists(string virtualPath); // check if a file exists
		void MoveFile(string fromVirtualPath, string destinationVirtualPath); // moves a file to a new path
		void DeleteFile(string virtualPath); // deletes a file
		void CopyFile(string fromVirtualPath, string destinationVirtualPath); // copies a file or directory to a new path

		Stream OpenFile(string virtualPath); // option 1: work against a file stream

		void WriteFile(string virtualPath, Stream inputStream); // option 2: creates or updates a file at the given path (simpler than 1?)
		void ReadFileContents(string virtualPath, Stream outputStream); // option 2: writes file content to the output stream

		bool DirectoryExists(string virtualPath); // check if a directory exists
		void MoveDirectory(string fromVirtualPath, string destinationVirtualPath); // moves a directory to a new path
		void DeleteDirectory(string virtualPath); // deletes a directory
		void CreateDirectory(string virtualPath); // creates a directory
	}
}