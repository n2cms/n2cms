using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using N2.Web;

namespace N2.Management.Api
{
    public class ApiHandlerDispatcher : IHttpHandler
    {
        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            var dir = N2.Web.Url.ResolveTokens("{ManagementUrl}/Api/");
            if (!context.Request.FilePath.StartsWith(dir, StringComparison.InvariantCultureIgnoreCase))
                return;

            var name = context.Request.FilePath.Substring(dir.Length).Replace(".ashx", "Handler");

            var handler = Context.Current.Container.ResolveAll<IApiHandler>().FirstOrDefault(h => h.GetType().Name == name);
            if (handler != null)
                handler.ProcessRequest(context.GetHttpContextBase());
        }
    }
}
