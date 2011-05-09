namespace N2.Web
{
	public static class Extensions
	{
		/// <summary>Converts a string to an <see cref="Url"/></summary>
		/// <param name="url">The url string.</param>
		/// <returns>The string parsed into an Url.</returns>
		public static Url ToUrl(this string url)
		{
			return Url.Parse(url);
		}
	}
}
