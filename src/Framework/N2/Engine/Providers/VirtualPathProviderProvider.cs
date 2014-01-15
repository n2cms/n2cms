using System.Collections.Generic;
using System.Web.Hosting;

namespace N2.Engine.Providers
{
    [Service(typeof(IProvider<VirtualPathProvider>))]
    public class VirtualPathProviderProvider : IProvider<VirtualPathProvider>
    {
        #region IProvider<VirtualPathProvider> Members

        public VirtualPathProvider Get()
        {
            return HostingEnvironment.VirtualPathProvider;
        }

        public IEnumerable<VirtualPathProvider> GetAll()
        {
            return new[] { HostingEnvironment.VirtualPathProvider };
        }

        #endregion
    }
}
