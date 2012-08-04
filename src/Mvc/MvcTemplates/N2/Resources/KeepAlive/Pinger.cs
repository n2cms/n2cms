using System;
using System.Configuration;
using N2.Plugin.Scheduling;
using System.Diagnostics;
using N2.Engine;
using System.Net;
using N2.Web;
using N2.Configuration;
using System.Security;

namespace N2.Edit.KeepAlive
{
    [ScheduleExecution(1, TimeUnit.Minutes)]
    public class Pinger : ScheduledAction
    {
		private readonly Engine.Logger<Pinger> logger;

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

            if (Debugger.IsAttached)
                return;
            
            try
            {
                Url url = Url.ServerUrl;
                if (url == null)
                    return;

                using (WebClient wc = new WebClient())
                {
                    wc.Headers["N2KeepAlive"] = "true";
                    url = url.SetPath(config.Scheduler.KeepAlivePath.ResolveUrlTokens());
                    string response = wc.DownloadString(url);
                    Debug.WriteLine("Ping " + url + ": " + response);
                    logger.Debug("Ping " + url + ": " + response);
                }
            }
			catch(SecurityException ex)
			{
				N2.Engine.Logger.Warn("Stopping keep-alive after exception (probably medium trust environemtn): ", ex);
				Repeat = Repeat.Once;
			}
        }
    }
}
