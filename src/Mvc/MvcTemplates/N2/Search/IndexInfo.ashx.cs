using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Web;
using N2.Persistence.Search;

namespace N2.Management.Search
{
    /// <summary>
    /// Provides info about the index to scripts.
    /// </summary>
    public class IndexInfo : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            if (!N2.Context.Current.SecurityManager.IsAdmin(context.User))
                throw new N2.Security.PermissionDeniedException();

            context.Response.ContentType = "application/json";
            new
            {
                Statistics = N2.Context.Current.Resolve<IContentIndexer>().GetStatistics(),
                Status = N2.Context.Current.Resolve<IAsyncIndexer>().GetCurrentStatus()
            }.ToJson(context.Response.Output);
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
