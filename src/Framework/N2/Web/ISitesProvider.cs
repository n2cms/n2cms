using System.Collections.Generic;

namespace N2.Web
{
    /// <summary>
    /// Classes implementing this interface can serve as providers for sites 
    /// when using the multiple site url parser.
    /// </summary>
    public interface ISitesProvider
    {
        /// <summary>Gets site roots configured for this installation.</summary>
        /// <returns>An enumeration of sites.</returns>
        IEnumerable<Site> GetSites();
    }
}
