using N2.Configuration;
using N2.Persistence.Search;
using N2.Web;
using System;
using System.Collections.Generic;
using System.Configuration;
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
			Console.WriteLine("Press enter key to exit");
			Console.ReadLine();
			server.Stop();
		}


	}
}
