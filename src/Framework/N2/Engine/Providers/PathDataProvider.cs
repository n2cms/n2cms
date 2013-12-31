using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Web;

namespace N2.Engine.Providers
{
    [Service(typeof(IProvider<PathData>))]
    public class PathDataProvider : IProvider<PathData>
    {
        IWebContext webContext;

        public PathDataProvider(IWebContext webContext)
        {
            this.webContext = webContext;
        }

        #region IProvider<PathData> Members

        public PathData Get()
        {
            return webContext.CurrentPath;
        }

        public IEnumerable<PathData> GetAll()
        {
            return new[] { Get() };
        }

        #endregion
    }
}
