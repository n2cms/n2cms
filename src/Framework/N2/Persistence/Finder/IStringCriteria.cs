namespace N2.Persistence.Finder
{
    /// <summary>
    /// Classes implementing this interface provides functionality to add 
    /// criteras for string equality and like operations.
    /// </summary>
    public interface IStringCriteria : ICriteria<string>
    {
        /// <summary>SQL "Like" expression</summary>
        IQueryAction Like(string value);

        /// <summary>SQL "Not Like" expression</summary>
        IQueryAction NotLike(string value);
    }
}
