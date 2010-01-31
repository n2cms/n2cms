using N2.Engine;

namespace N2.Web
{
	/// <summary>
	/// Resolves and constructs the controller used to further control 
	/// various operations to content items.
	/// </summary>
	public interface IRequestDispatcher
	{
		/// <summary>Resolves the adapter for the current Url.</summary>
		/// <returns>A suitable adapter for the current Url.</returns>
		T ResolveAdapter<T>(ContentItem item) where T : AbstractContentAdapter;

		/// <summary>Resolves the adapter for the current Url.</summary>
		/// <returns>A suitable adapter for the current Url.</returns>
		PathData GetCurrentPath();
	}
}