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
			var engine = N2.Context.Current;

            if (!engine.SecurityManager.IsAdmin(context.User))
                throw new N2.Security.PermissionDeniedException();

            context.Response.ContentType = "application/json";
            new
            {
				IndexerType = engine.Resolve<IIndexer>().GetType().Name,
                Statistics = engine.Resolve<IContentIndexer>().GetStatistics(),
                Status = engine.Resolve<IAsyncIndexer>().GetCurrentStatus()
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
