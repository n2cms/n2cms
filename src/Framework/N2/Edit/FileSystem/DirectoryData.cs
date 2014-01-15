namespace N2.Edit.FileSystem
{
    /// <summary>
    /// Represents a directory provided through the <see cref="IFileSystem"/>.
    /// </summary>
    public class DirectoryData : AbstractFileSystemItem
    {
        public static DirectoryData Virtual(string virtualDir)
        {
            return new DirectoryData { Name = virtualDir.Trim('~', '/'), VirtualPath = virtualDir };
        }
    }
}
