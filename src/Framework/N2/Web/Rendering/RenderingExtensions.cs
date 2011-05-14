namespace N2.Web.Rendering
{
	public static class RenderingExtensions
	{
		public static string TextUntil(this string text, params char[] untilAny)
		{
			return TextUntil(text, 0, untilAny);
		}
		public static string TextUntil(this string text, int startIndex, params char[] untilAny)
		{
			int index = text.IndexOfAny(untilAny, startIndex);
			if (index < 0)
				return text;
			return text.Substring(startIndex, index - startIndex);
		}
		public static string TextUntil(this string text, string untilFirst)
		{
			int index = text.IndexOf(untilFirst);
			if (index < 0)
				return text;
			return text.Substring(0, index);
		}
	}
}
