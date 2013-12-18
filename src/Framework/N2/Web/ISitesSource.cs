using System.Collections.Generic;

namespace N2.Web
{
    /// <summary>
    /// This interface is used by the <see cref="DynamicSitesProvider"/> to 
    /// retrieve available sites.
    /// </summary>
    public interface ISitesSource
    {
        /// <summary>Gets sites specified by a site node.</summary>
        /// <returns>An enumeration of sites.</returns>
        /// <remarks>The returned sites are cached.</remarks>
        IEnumerable<Site> GetSites();
    }
}
