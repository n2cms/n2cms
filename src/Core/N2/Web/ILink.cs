namespace N2.Web
{
	/// <summary>
	/// Represents a link to somewhere.
	/// </summary>
	public interface ILink
	{
		string Text { get; }
		string Title { get; }
		string Target { get; }
		string Href { get; }
	}
}
