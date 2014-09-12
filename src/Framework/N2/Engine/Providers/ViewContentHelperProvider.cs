using System;
using System.Collections.Generic;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web;
using System.Web.Routing;
using N2.Web;
using N2.Web.Mvc;

namespace N2.Engine.Providers
{
    [Service(typeof(IProvider<ViewContentHelper>))]
    public class ViewContentHelperProvider : IProvider<ViewContentHelper>
    {
        IProvider<HtmlHelper> htmlHelperProvider;

        public ViewContentHelperProvider(IProvider<HtmlHelper> htmlHelperProvider)
        {
            this.htmlHelperProvider = htmlHelperProvider;
        }

        #region IProvider<ContentHelper> Members

        public ViewContentHelper Get()
        {
			if (htmlHelperProvider == null)
				throw new NullReferenceException("Failed to get view content because htmlHelperProvider was null.");
            return new ViewContentHelper(htmlHelperProvider.Get());
        }

        public IEnumerable<ViewContentHelper> GetAll()
        {
            return new[] { Get() };
        }

        #endregion
    }
}
