using N2.Plugin;
using N2.Engine.Globalization;

namespace N2.Edit.Globalization
{
	[AutoInitialize]
	public class LanguageInitializer : IPluginInitializer
	{
		public void Initialize(N2.Engine.IEngine engine)
		{
			engine.AddComponent("n2.languageGateway", typeof(ILanguageGateway), typeof(LanguageGateway));
			engine.AddComponent("n2.languageInterceptor", typeof(LanguageInterceptor));
		}
	}
}
