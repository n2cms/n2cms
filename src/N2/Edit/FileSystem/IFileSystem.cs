using System.Collections.Generic;
using System.IO;

namespace N2.Edit.FileSystem
{
	/// <summary>
	/// Abstracts operations against binary files.
	/// </summary>
	public interface IFileSystem
	{
		IEnumerable<string> GetFiles(string parentVirtualPath); // gets files in directory
		IEnumerable<string> GetDirectories(string parentVirtualPath); // gets directories in directory

		bool Exists(string virtualPath); // check if a file or directory exists
		void Delete(string virtualPath); // deletes a file or directory
		void Move(string fromVirtualPath, string toNewVirtualPath); // moves a file or directory to a new path
		void Copy(string fromVirtualPath, string toNewVirtualPath); // copies a file or directory to a new path
		void CreateDirectory(string virtualPath); // creates a directory

		Stream OpenFile(string virtualPath); // option 1: work against a file stream

		void CreateOrUpdateFile(string virtualPath, Stream inputStream); // option 2: creates or updates a file at the given path (perhaps simpler than 1)
		void WriteFileContents(string virtualPath, Stream outputStream); // option 2: writes file content to the output stream
	}
}