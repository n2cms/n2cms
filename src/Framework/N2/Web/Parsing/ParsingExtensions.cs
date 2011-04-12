using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Web.Parsing
{
	public static class ParsingExtensions
	{
		public static string StringJoin(this IEnumerable<string> strings)
		{
			var sb = new StringBuilder();
			foreach (var s in strings)
				sb.Append(s);
			return sb.ToString();
		}
	}
}
