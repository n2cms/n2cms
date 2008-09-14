using System;
using System.Collections.Generic;

namespace N2.Web
{
    /// <summary>
    /// Classes implementing this interface knows about available 
    /// <see cref="Site">Sites</see> and which one is the current
    /// based on the context.
    /// </summary>
	public interface IHost
	{
		Site CurrentSite { get; }
		Site DefaultSite { get; set; }
		IList<Site> Sites { get; }
        Site GetSite(Url host);

        void AddSites(IEnumerable<Site> iEnumerable);
        void ReplaceSites(IList<Site> newSites);
    }
}
