using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace N2.Web.Parsing
{
	[DebuggerDisplay("{Command} ({Tokens.Count}): {ToString()}")]
	public class Component
	{
		public string Command { get; set; }
		public string Argument { get; set; }
		public IList<Token> Tokens { get; set; }

		public IEnumerable<Component> Components { get; set; }

		internal static IEnumerable<Component> None = new Component[0];

		public override string ToString()
		{
			if (Tokens == null)
				return base.ToString();

			return Tokens.Select(t => t.Fragment).StringJoin();
		}
	}
}
