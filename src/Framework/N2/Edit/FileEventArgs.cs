using System;

namespace N2.Edit
{
    /// <summary>
    /// Conveys information about files and directories.
    /// </summary>
    public class FileEventArgs : EventArgs
    {
        public FileEventArgs(string virtualPath, string sourcePath)
        {
            VirtualPath = virtualPath;
            SourcePath = sourcePath;
        }

        /// <summary>The current path of the file or directory.</summary>
        public string VirtualPath { get; set; }
        /// <summary>The source path of the file or directory.</summary>
        public string SourcePath { get; set; }
    }
}
