using N2.Engine;
using N2.Parts.Web;
using N2.Plugin;

namespace N2.Parts
{
	[AutoInitialize]
	public class PartInitializer : IPluginInitializer
	{
		public void Initialize(IEngine engine)
		{
			engine.AddComponent("n2.parts.createHandler", typeof(CreateUrlProvider));
			engine.AddComponent("n2.parts.deleteHandler", typeof(ItemDeleter));
			engine.AddComponent("n2.parts.editHandler", typeof(EditUrlProvider));
			engine.AddComponent("n2.parts.moveHandler", typeof(ItemMover));
			engine.AddComponent("n2.parts.copyHandler", typeof(ItemCopyer));
		}
	}
}
