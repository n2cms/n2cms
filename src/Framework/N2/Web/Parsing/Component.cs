using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Web.Parsing
{
	public class Component
	{
		public string Command { get; set; }
		public string Data { get; set; }
		public ICollection<Token> Tokens { get; set; }
	}
}
