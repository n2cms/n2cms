namespace N2.Edit.FileSystem
{
    /// <summary>
    /// Finds the default folder associated with an item.
    /// </summary>
    public interface IDefaultDirectory
    {
        string GetDefaultDirectory(ContentItem item);
    }
}
