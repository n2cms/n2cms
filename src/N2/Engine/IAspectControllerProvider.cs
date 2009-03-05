using System.Text;
using N2.Web;

namespace N2.Engine
{
	public interface IAspectControllerProvider
	{
		/// <summary>Resolves the controller for the current Url.</summary>
		/// <returns>A suitable controller for the given Url.</returns>
		T ResolveAspectController<T>(PathData path) where T : class, IAspectController;

		/// <summary>Adds controller descriptors to the list of descriptors. This is typically auto-wired using the [Controls] attribute.</summary>
		/// <param name="descriptorToAdd">The controller descriptors to add.</param>
		void RegisterAspectController(params IControllerDescriptor[] descriptorToAdd);
	}
}
