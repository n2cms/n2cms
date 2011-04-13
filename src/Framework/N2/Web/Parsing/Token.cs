using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace N2.Web.Parsing
{
	[DebuggerDisplay("[{Type} Fragment={Fragment}]")]
	public class Token
	{
		public int Index { get; set; }
		public TokenType Type { get; set; }
		public string Fragment { get; set; }
	}
}
