namespace N2.Persistence
{
    /// <summary>
    /// Content items implementing this interface are responsible for their 
    /// own persistence.
    /// </summary>
    public interface IActiveContent
    {
        void Save();
        void Delete();
        void MoveTo(ContentItem destination);
        ContentItem CopyTo(ContentItem destination);
    }
}
