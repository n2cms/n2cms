using System.Collections.Generic;
using N2.Web;

namespace N2.Tests.Fakes
{
    public class StaticSitesProvider : ISitesProvider
    {
        IEnumerable<Site> sites;

        public StaticSitesProvider(IEnumerable<Site> sites)
        {
            this.sites = sites;
        }

        public IEnumerable<Site> GetSites()
        {
            return sites;
        }
    }
}
