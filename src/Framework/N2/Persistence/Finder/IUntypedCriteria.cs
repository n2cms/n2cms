using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.Finder
{
    /// <summary>
    /// Classes implementing this interface provides options to add criteras 
    /// for various comparisons on item persistable properties and details.
    /// </summary>
    public interface IUntypedCriteria
    {
        /// <summary>Equal to.</summary>
        IQueryAction Eq<T>(T value);

        /// <summary>Not equal to.</summary>
        IQueryAction NotEq<T>(T value);

        /// <summary>Greater than.</summary>
        IQueryAction Gt<T>(T value);

        /// <summary>Greater or equal than.</summary>
        IQueryAction Ge<T>(T value);

        /// <summary>Less than.</summary>
        IQueryAction Lt<T>(T value);

        /// <summary>Less or equal than.</summary>
        IQueryAction Le<T>(T value);

        /// <summary>Between lowerBound and upperBound.</summary>
        IQueryAction Between<T>(T lowerBound, T upperBound);

        /// <summary>SQL "Like" expression</summary>
        IQueryAction Like(string value);

        /// <summary>SQL "Not Like" expression</summary>
        IQueryAction NotLike(string value);

        IQueryAction In<T>(params T[] values);
    }
}
