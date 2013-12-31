namespace N2.Persistence.Finder
{
    /// <summary>
    /// Classes implementing this interface provides methods to continue the 
    /// query building or end the current query.
    /// </summary>
    public interface IQueryAction : IQueryEnding
    {
        /// <summary>Continues the building of a query combining previous expressions with the and operator.</summary>
        IQueryBuilder And { get; }

        /// <summary>Continues the building of a query combining previous expressions with the or operator.</summary>
        IQueryBuilder Or { get; }

        /// <summary>"Closes the bracket" i.e. end a group of criterias that are evaluated together.</summary>
        IQueryAction CloseBracket();
    }
}
