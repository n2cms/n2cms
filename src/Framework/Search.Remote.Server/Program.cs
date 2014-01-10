using N2.Configuration;
using N2.Persistence.Search;
using N2.Web;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;

namespace N2.Search.Remote.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!Environment.UserInteractive)
            {
                ServiceBase.Run(new WindowsService());
                return;
            }

            var server = new IndexerServer();

            server.Start();
            Console.WriteLine("Listening on " + server.UriPrefix + ". Type \"exit\" and press enter to exit");
            string command = "";
            do
            {
                command = Console.ReadLine();
                if ("debug".Equals(command, StringComparison.InvariantCultureIgnoreCase))
                    Trace.Listeners.Add(new ConsoleWriterTraceListener());
                else if ("exit".Equals(command, StringComparison.InvariantCultureIgnoreCase))
                    break;
                else if ("cls".Equals(command, StringComparison.InvariantCultureIgnoreCase))
                    Console.Clear();
            } while (true);

            Console.WriteLine("Exiting...");
            server.Stop();
        }


    }
    public class ConsoleWriterTraceListener : TraceListener
    {
        public override void Write(string message)
        {
            Console.Write(message);
        }

        public override void WriteLine(string message)
        {
            Console.WriteLine(message);
        }
    }

}
