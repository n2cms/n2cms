using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BuildSupport
{
	class Program
	{
		static void Main(string[] args)
		{
			var methods = typeof(Commands).GetMethods();
			var method = methods.FirstOrDefault(m => m.Name == args.FirstOrDefault());
			if (method == null)
			{
				Console.Error.WriteLine("Invalid command argument '" + args.FirstOrDefault() + "'. Available commands: " + string.Join("", methods.Select(m => m.Name).Except(typeof(object).GetMethods().Select(m => m.Name)).Select(n => Environment.NewLine + " - " + n).ToArray()));
				Environment.ExitCode = 1;
				return;
			}
			var parameters = method.GetParameters();
			if (parameters.Length != (args.Length - 1))
			{
				Console.Error.WriteLine("Invalid number of command arguments (was " + (args.Length - 1) + " needs " + parameters.Length + "): " + string.Join("", parameters.Select(p => Environment.NewLine + " - " + p.Name).ToArray()));
				Environment.ExitCode = 1;
				return;
			}
			method.Invoke(new Commands(), args.Skip(1).ToArray());
		}
	}
}
