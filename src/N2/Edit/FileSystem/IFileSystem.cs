using System.Collections.Generic;
using System.IO;

namespace N2.Edit.FileSystem
{
	/// <summary>
	/// Abstracts operations against binary files.
	/// </summary>
	public interface IFileSystem
	{
		IEnumerable<string> GetFiles(string parentPath); // gets files in directory
		IEnumerable<string> GetDirectories(string parentPath); // gets directories in directory

		bool Exists(string path); // check if a file or directory exists
		void Delete(string path); // deletes a file or directory
		void Move(string fromPath, string toNewPath); // moves a file or directory to a new path
		void Copy(string fromPath, string toNewPath); // copies a file or directory to a new path
		void CreateDirectory(string path); // creates a directory
		void CreateOrUpdateFile(string path, Stream inputStream); // creates or updates a file at the given path
		void WriteFileContents(string path, Stream outputStream); // writes file content to the output stream
	}
}