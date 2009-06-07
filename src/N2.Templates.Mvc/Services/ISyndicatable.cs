using System;

namespace N2.Templates.Mvc.Services
{
	/// <summary>
	/// Items implementing this interface provide the information needed by the
	/// <see cref="RssWriter"/> to write a feed.
	/// </summary>
	public interface ISyndicatable
	{
		string Title { get; }
		string Url { get; }
		string Summary { get; }
		DateTime? Published { get; }
	}
}