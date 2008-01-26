using N2.Plugin;

namespace N2.Edit.Versions
{
	/// <summary>
	/// Enables versioning if the editor interface is available.
	/// </summary>
	[AutoInitialize]
	public class VersioningEnabler : IPluginInitializer
	{
		public void Initialize(Engine.IEngine engine)
		{
			engine.EditManager.EnableVersioning = true;
		}
	}
}
