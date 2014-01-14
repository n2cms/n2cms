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

			string instance = "Pages";
			var commands = new Dictionary<string, Action<string>>(StringComparer.InvariantCultureIgnoreCase);

			commands["debug"] = (arg) => Trace.Listeners.Add(new ConsoleWriterTraceListener());
			commands["exit"] = (arg) => { server.Stop(); Environment.Exit(-1); };
			commands["cls"] = (arg) => Console.Clear();
			commands["search"] = (arg) => Search(server, instance, arg);
			commands["count"] = (arg) => Count(server, instance);
			commands["instance"] = (arg) => { if (string.IsNullOrEmpty(arg)) Console.WriteLine("Current instance: " + instance); else instance = arg; };
			commands["help"] = (arg) => { Console.WriteLine("Available commands: "); foreach (var command in commands) Console.WriteLine(" * " + command.Key); };

			try
			{	        
				do
				{
					Console.Write(">");
					var input = Console.ReadLine();
					var command = input.Split(' ')[0];
					var argument = input.Substring(command.Length);

					Action<string> action;
					if (commands.TryGetValue(command, out action))
						action(argument);
					else if (!string.IsNullOrEmpty(command))
						Console.WriteLine("Invalid command");
					
				} while (true);

			}
			finally
			{
				Console.WriteLine("Exiting...");
				server.Stop();
			}
        }

		private static void Count(IndexerServer server, string instance)
		{
			Console.WriteLine("Number of documents on instance " + instance + ": " + server.Statistics(instance).TotalDocuments);
		}

		private static void Search(IndexerServer server, string instance, string query)
		{
			Console.WriteLine("Searching for " + query);
			foreach (var result in server.Search(instance, query).Hits)
			{
				Console.WriteLine(result.Content.ID + ":\t" + result.Title);
			}
		}
    }

    public class ConsoleWriterTraceListener : TraceListener
    {
        public override void Write(string message)
        {
			using (OverrideColor(ConsoleColor.DarkGray))
			{
				Console.Write(message);
			}
        }

        public override void WriteLine(string message)
        {
			using (OverrideColor(ConsoleColor.DarkGray))
			{
				Console.WriteLine(message);
			}
        }

		private IDisposable OverrideColor(ConsoleColor newColor)
		{
			var reset = new ColorReset(Console.ForegroundColor);
			Console.ForegroundColor = newColor;
			return reset;
		}

		class ColorReset : IDisposable
		{
			private ConsoleColor resetTo;

			public ColorReset(ConsoleColor resetToOnDispose)
			{
				this.resetTo = resetToOnDispose;
			}
			public void Dispose()
			{
				Console.ForegroundColor = resetTo;
			}
		}
    }

}
