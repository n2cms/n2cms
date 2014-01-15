using System.Collections.Generic;
using System.Web.Mvc;

namespace N2.Engine.Providers
{
    [Service(typeof(IProvider<ViewEngineCollection>))]
    public class ViewEngineCollectionProvider : IProvider<ViewEngineCollection>
    {
        #region IProvider<ViewEngineCollection> Members

        public ViewEngineCollection Get()
        {
            return ViewEngines.Engines;
        }

        public IEnumerable<ViewEngineCollection> GetAll()
        {
            return new[] { ViewEngines.Engines };
        }

        #endregion
    }
}
