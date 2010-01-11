using N2.Plugin;

namespace N2.Edit.Trash
{
	[AutoInitialize]
	public class TrashcanInitializer : IPluginInitializer
	{
		public void Initialize(Engine.IEngine engine)
		{
			engine.AddComponent("n2.trash", typeof(ITrashHandler), typeof(TrashHandler));
			engine.AddComponent("n2.deleteInterceptor", typeof(DeleteInterceptor));
		}
	}
}
