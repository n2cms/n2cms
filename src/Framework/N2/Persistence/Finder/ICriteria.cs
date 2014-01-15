namespace N2.Persistence.Finder
{
    /// <summary>
    /// Classes implementing this interface provides functionality to add 
    /// criteras for equality.
    /// </summary>
    public interface ICriteria<T>
    {
        /// <summary>Equal to.</summary>
        IQueryAction Eq(T value);

        /// <summary>Not equal to.</summary>
        IQueryAction NotEq(T value);

        IQueryAction In(params T[] anyOf);

        /// <summary>The parameter is or isn't null.</summary>
        /// <param name="isNull">True if the parameter should be null.</param>
        /// <returns></returns>
        IQueryAction IsNull(bool isNull = true);
    }
}
