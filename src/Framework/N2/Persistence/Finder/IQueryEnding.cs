using System.Collections.Generic;
using N2.Collections;
using System;

namespace N2.Persistence.Finder
{
    /// <summary>
    /// Classes implementing this interface provide the last options before the
    /// end of the query.
    /// </summary>
    public interface IQueryEnding : IQueryEnd
    {
        /// <summary>Orders result by the given expression.</summary>
        IOrderBy OrderBy { get; }

        [Obsolete("Querying previous versions is no longer supported", true)]
        /// <summary>Defines wether previous versions are included in the query. By default previous versions are excluded.</summary>
        IQueryEnding PreviousVersions(VersionOption option);

        /// <summary>Sets the first result index for the query to return.</summary>
        IQueryEnding FirstResult(int firstResultIndex);

        /// <summary>Sets the maximum amount of results for the query to return.</summary>
        IQueryEnding MaxResults(int maxResults);

        /// <summary>Sets filters applied to the result set.</summary>
        /// <param name="filters">The filters that are applied.</param>
        IQueryEnding Filters(params ItemFilter[] filters);

        /// <summary>Sets filters applied to the result set.</summary>
        /// <param name="filters">The filters that are applied.</param>
        IQueryEnding Filters(IEnumerable<ItemFilter> filters);
    }
}
