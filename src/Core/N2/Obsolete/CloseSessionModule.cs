using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Diagnostics;
using System.ComponentModel;

namespace N2.Web
{
    /// <summary>A HttpModule that ensures that the NHibernate session is closed.</summary>
	[Obsolete("Use InitializerModule instead.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class CloseSessionModule : IHttpModule
    {
        public void Dispose()
        {
		}

        public void Init(HttpApplication context)
        {
			throw new N2Exception("The CloseSessionModule has been deprecated, replace it with the N2.Web.InitializerModule in web.config's httpModules section.");
		}
    }
}
