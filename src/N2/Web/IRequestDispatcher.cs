using N2.Engine;

namespace N2.Web
{
	/// <summary>
	/// Resolves and constructs the controller used to further control 
	/// various aspects related to content items.
	/// </summary>
	public interface IRequestDispatcher
	{
		/// <summary>Resolves the controller for the current Url.</summary>
		/// <returns>A suitable controller for the given Url.</returns>
		T ResolveAspectController<T>() where T : class, IAspectController;

		/// <summary>Adds controller descriptors to the list of descriptors. This is typically auto-wired using the [Controls] attribute.</summary>
		/// <param name="descriptorToAdd">The controller descriptors to add.</param>
		void RegisterAspectController(params IControllerDescriptor[] descriptorToAdd);
	}
}