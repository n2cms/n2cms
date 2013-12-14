namespace N2.Persistence.Finder
{
    /// <summary>
    /// Classes implementing this interface provides functionality to add a 
    /// criteria with operations such as greater than and between.
    /// </summary>
    /// <typeparam name="T">The type of value to search for.</typeparam>
    public interface IComparisonCriteria<T> : ICriteria<T>
    {
        /// <summary>Greater than.</summary>
        IQueryAction Gt(T value);

        /// <summary>Greater or equal than.</summary>
        IQueryAction Ge(T value);
        
        /// <summary>Less than.</summary>
        IQueryAction Lt(T value);
        
        /// <summary>Less or equal than.</summary>
        IQueryAction Le(T value);
        
        /// <summary>Between lowerBound and upperBound.</summary>
        IQueryAction Between(T lowerBound, T upperBound);
    }
}
