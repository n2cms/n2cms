namespace N2.Persistence.Finder
{
    /// <summary>
    /// Classes implementing this interface provides options to add criteras 
    /// for various comparisons on item persistable properties.
    /// </summary>
    public interface IPropertyCriteria : IUntypedCriteria
    {
        /// <summary>Presence of a detail.</summary>
        /// <param name="isNull">True if the detail is expected to be present.</param>
        IQueryAction Null(bool isNull);
    }
}
