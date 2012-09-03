
using N2.Persistence.Search;
namespace N2.Definitions
{
	/// <summary>
	/// Marker interface used to denote parts. This interface allows collaboration
	/// between modules that doesn't know about each other.
	/// </summary>
	[SearchableType]
	public interface IPart
	{
		string ZoneName { get; }
	}
}
