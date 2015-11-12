using System;
using System.Collections.Generic;
using System.IO;

namespace N2.Edit.FileSystem
{
    /// <summary>
    /// Abstracts operations against binary files.
    /// </summary>
    public interface IFileSystem
    {
        /// <summary>Gets files in directory.</summary>
        /// <param name="parentVirtualPath">The path to the directory.</param>
        /// <returns>An enumerations of files in the directory.</returns>
        IEnumerable<FileData> GetFiles(string parentVirtualPath); 

        /// <summary>Gets file data.</summary>
        /// <param name="virtualPath">The path to the file.</param>
        /// <returns>A file data object.</returns>
        FileData GetFile(string virtualPath);

        /// <summary>Gets directories of a directory.</summary>
        /// <param name="parentVirtualPath">The path to the directory whose child directories to get.</param>
        /// <returns>An enumeration of directories.</returns>
        IEnumerable<DirectoryData> GetDirectories(string parentVirtualPath);

        /// <summary>Gets a directory data.</summary>
        /// <param name="virtualPath">The path of the directory to get.</param>
        /// <returns>A directory data object.</returns>
        DirectoryData GetDirectory(string virtualPath);

        /// <summary>Searches for files in all Upload Directories.</summary>
        /// <param name="query">The search term</param>
        /// <param name="uploadDirectories">All Upload Directories</param>
        /// <returns>An enumeration of files matching the query.</returns>
        IEnumerable<FileData> SearchFiles(string query, List<Collections.HierarchyNode<ContentItem>> uploadDirectories);

        /// <summary>Checks if a file exists.</summary>
        /// <param name="virtualPath">The path of the file to check.</param>
        /// <returns>True if the file exists in the file system.</returns>
        bool FileExists(string virtualPath);

        /// <summary>Moves a file to a new location.</summary>
        /// <param name="fromVirtualPath">The file path where the file is located.</param>
        /// <param name="destinationVirtualPath">The path the file should assume after beeing moved.</param>
        void MoveFile(string fromVirtualPath, string destinationVirtualPath);

        /// <summary>Delets a file from the file system.</summary>
        /// <param name="virtualPath">The path of the file to delete.</param>
        void DeleteFile(string virtualPath);

        /// <summary>Copies a file.</summary>
        /// <param name="fromVirtualPath">The path where the file is located.</param>
        /// <param name="destinationVirtualPath">The path the copy should assume when.</param>
        void CopyFile(string fromVirtualPath, string destinationVirtualPath);

        /// <summary>Opens a read-write file stream to a file.</summary>
        /// <param name="virtualPath">The path where the file is located.</param>
        /// <param name="readOnly">Return a read-only stream if the underlying implementation supports it.</param>
        /// <returns>A file stream.</returns>
        Stream OpenFile(string virtualPath, bool readOnly = false); // option 1: work against a file stream

        /// <summary>Creates or overwrites a file at the given path.</summary>
        /// <param name="virtualPath">The path of the file to create.</param>
        /// <param name="inputStream">An input stream of the file contents.</param>
        void WriteFile(string virtualPath, Stream inputStream); // option 2: creates or updates a file at the given path (simpler than 1?)

        /// <summary>Read file contents to a stream.</summary>
        /// <param name="virtualPath">The path of the file to read.</param>
        /// <param name="outputStream">The stream to which the file contents should be written.</param>
        void ReadFileContents(string virtualPath, Stream outputStream); // option 2: writes file content to the output stream

        /// <summary>Checks the existence of a directory.</summary>
        /// <param name="virtualPath">The directory to check.</param>
        /// <returns>True if the directory exists.</returns>
        bool DirectoryExists(string virtualPath);

        /// <summary>Moves a directory.</summary>
        /// <param name="fromVirtualPath">The original directory path.</param>
        /// <param name="destinationVirtualPath">The path the directory should assume after beeing moved.</param>
        void MoveDirectory(string fromVirtualPath, string destinationVirtualPath);

        /// <summary>Deletes a directory including all files and sub-directories.</summary>
        /// <param name="virtualPath">The path of the directory to remove.</param>
        void DeleteDirectory(string virtualPath);

        /// <summary>Creates a directory.</summary>
        /// <param name="virtualPath">The directory path to create.</param>
        void CreateDirectory(string virtualPath);

        event EventHandler<FileEventArgs> FileWritten;
        event EventHandler<FileEventArgs> FileCopied;
        event EventHandler<FileEventArgs> FileMoved;
        event EventHandler<FileEventArgs> FileDeleted;
        event EventHandler<FileEventArgs> DirectoryCreated;
        event EventHandler<FileEventArgs> DirectoryMoved;
        event EventHandler<FileEventArgs> DirectoryDeleted;
    }
}
