using N2.Plugin;

namespace N2.Edit.LinkTracker
{
	[AutoInitialize]
	public class LinkTrackerInitializer : IPluginInitializer
	{
		public void Initialize(Engine.IEngine engine)
		{
			engine.AddComponent("n2.linkTracker", typeof(Tracker));
		}
	}
}
