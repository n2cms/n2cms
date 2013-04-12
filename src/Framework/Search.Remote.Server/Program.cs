using N2.Configuration;
using N2.Persistence.Search;
using N2.Web;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;

namespace N2.Search.Remote.Server
{
	class Program
	{
		static void Main(string[] args)
		{
			var server = new IndexerServer();
			
			server.Start();

			Console.CancelKeyPress += delegate
			{
				server.Stop();
			};

			Console.WriteLine("Listening on " + server.UriPrefix + ". Press enter key to exit");
			string command = "";
			do
			{
				command = Console.ReadLine();
				if ("debug".Equals(command, StringComparison.InvariantCultureIgnoreCase))
					Trace.Listeners.Add(new ConsoleWriterTraceListener());
				if ("exit".Equals(command, StringComparison.InvariantCultureIgnoreCase))
					break;
			} while (!string.IsNullOrEmpty(command));
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
