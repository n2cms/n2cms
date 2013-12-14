using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Definitions;

namespace N2.Management.Resources
{
    /// <summary>
    /// Summary description for Redirect
    /// </summary>
    public class RedirectHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            var redirect = N2.Content.Current.Page as IRedirect;
            if (redirect != null && redirect.RedirectTo != null)
            {
                context.Response.Status = "301 Moved Permanently";
                context.Response.AddHeader("Location", redirect.RedirectTo.Url);
            }
            else
            {
                context.Response.Status = "410 Gone";
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
