using System.Collections.Generic;
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

		public static bool IsWithinBounds<T>(this ICollection<T> collection, int index)
		{
			return index >= 0 && index < collection.Count;
		}
	}
}
