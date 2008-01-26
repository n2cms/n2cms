using N2.Plugin;

namespace N2.Trashcan
{
	[AutoInitialize]
	public class TrashcanInitializer : IPluginInitializer
	{
		public void Initialize(Engine.IEngine engine)
		{
			engine.AddComponent("n2.deleteInterceptor", typeof(DeleteInterceptor));
			engine.AddComponent("n2.trash", typeof(TrashHandler));
		}
	}
}
