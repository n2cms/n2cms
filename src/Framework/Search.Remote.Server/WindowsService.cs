using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace N2.Search.Remote.Server
{
    public class WindowsService : ServiceBase
    {
        private IndexerServer server;

        public WindowsService()
        {
            ServiceName = "N2 Search Service";
            EventLog.Log = "Application";

            this.CanHandlePowerEvent = true;
            this.CanHandleSessionChangeEvent = true;
            this.CanPauseAndContinue = true;
            this.CanShutdown = true;
            this.CanStop = true;

            server = new IndexerServer();
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);

            server.Start();
        }

        protected override void OnStop()
        {
            server.Stop();

            base.OnStop();
        }

        protected override void OnPause()
        {
            server.Stop();

            base.OnPause();
        }

        protected override void OnContinue()
        {
            server.Start();

            base.OnContinue();
        }
    }
}
