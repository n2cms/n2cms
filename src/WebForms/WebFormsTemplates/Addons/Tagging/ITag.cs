namespace N2.Addons.Tagging
{
    public interface ITag
    {
        string Title { get; }
        int ReferenceCount { get; }
        IGroup Category { get; }
    }
}
