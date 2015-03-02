using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Edit;
using N2.Engine;
using N2.Web;

namespace N2.Management.Installation
{
    /// <summary>
    /// Summary description for UpgradeVersion
    /// </summary>
    public class UpgradeVersionHandler : IHttpHandler
    {
        private IEngine Engine
        {
            get { return Context.Current; }
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            try
            {
                var version = Engine.Persister.Get(int.Parse(context.Request[PathData.ItemQueryKey]));
                if (!version.VersionOf.HasValue)
                {
                    context.Response.Write(new { success = false, message = "Item #" + version.ID + " is not a version." }.ToJson());
                    return;
                }
                if (!version.IsPage)
                {
                    context.Response.Write(new { success = true, master = new { id = version.VersionOf.ID }, message = "Part version removed" }.ToJson());
					Engine.Persister.Delete(version);
                    return;
                }

                var newVersion = Engine.Resolve<UpgradeVersionWorker>().UpgradeVersion(version);
                var result = new { success = true, master = new { id = newVersion.Master.ID }, version = new { id = newVersion.ID, index = newVersion.VersionIndex } };
                context.Response.Write(result.ToJson());
            }
            catch (Exception ex)
            {
                new Logger<UpgradeVersionHandler>().Error("Error migrating #" + context.Request[PathData.ItemQueryKey], ex);
                context.Response.Write(new { success = false, message = ex.Message, stacktrace = ex.ToString() }.ToJson());
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
