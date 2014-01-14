using System.Collections.Generic;
using System.Web;

namespace N2.Engine.Providers
{
    [Service(typeof(IProvider<HttpContextBase>))]
    public class HttpContextProvider : IProvider<HttpContextBase>
    {
        #region IProvider<HttpContextBase> Members

        public HttpContextBase Get()
        {
            if (HttpContext.Current == null)
                return null;
            return new HttpContextWrapper(HttpContext.Current);
        }

        public IEnumerable<HttpContextBase> GetAll()
        {
            return new[] { Get() };
        }

        #endregion
    }
}
