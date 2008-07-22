using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using N2.Plugin.Scheduling;
using System.Diagnostics;
using N2.Engine;
using System.Net;
using N2.Web;
using N2.Configuration;

namespace N2.Edit.KeepAlive
{
    [ScheduleExecution(1, TimeUnit.Minutes)]
    public class Pinger : ScheduledAction
    {
        IEngine engine = null;
        EngineSection config = null;
        public override void Execute()
        {
            if (engine == null) engine = N2.Context.Current;
            if (config == null) config = ConfigurationManager.GetSection("n2/engine") as EngineSection;

            if (config == null || !config.Scheduler.KeepAlive)
            {
                Repeat = Repeat.Once;
                return;
            }
            
            try
            {
                Url url = Url.ServerUrl;
                if (url == null)
                    return;

                using (WebClient wc = new WebClient())
                {
                    wc.Headers["N2KeepAlive"] = "true";
                    string response = wc.DownloadString(url.SetPath(config.Scheduler.KeepAlivePath));
                    Debug.WriteLine("Ping " + url + ": " + response);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceWarning(ex.ToString());
            }
        }
    }
}
