using System.Text;
using N2.Web;

namespace N2.Engine
{
	public interface IContentAdapterProvider
	{
		/// <summary>Resolves the controller for the current Url.</summary>
		/// <returns>A suitable controller for the given Url.</returns>
		T ResolveAdapter<T>(PathData path) where T : class, IContentAdapter;

		/// <summary>Adds controller descriptors to the list of descriptors. This is typically auto-wired using the [Controls] attribute.</summary>
		/// <param name="descriptorsToAdd">The controller descriptors to add.</param>
		void RegisterAdapter(params IAdapterDescriptor[] descriptorsToAdd);
	}
}
