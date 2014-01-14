using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace N2.Web.Mvc
{
    /// <summary>
    /// A view location cache that takes theme into consideration.
    /// </summary>
    public class ThemeViewLocationCache : IViewLocationCache
    {
        IViewLocationCache inner = new DefaultViewLocationCache();

        string IViewLocationCache.GetViewLocation(System.Web.HttpContextBase httpContext, string key)
        {
            return inner.GetViewLocation(httpContext, httpContext.GetTheme() + key);
        }

        void IViewLocationCache.InsertViewLocation(System.Web.HttpContextBase httpContext, string key, string virtualPath)
        {
            inner.InsertViewLocation(httpContext, httpContext.GetTheme() + key, virtualPath);
        }
    }
}
