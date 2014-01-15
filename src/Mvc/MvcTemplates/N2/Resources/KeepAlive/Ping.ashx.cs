using System;
using System.Collections.Generic;
using System.Web;

namespace N2.Edit.KeepAlive
{
    public class Ping : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write("Pong");
        }

        public bool IsReusable
        {
            get { return true; }
        }
    }
}
