using System.Web;
using N2.MediumTrust.Engine;
using N2.Engine;

namespace N2.MediumTrust.Web
{
	public class InitializerModule : IHttpModule
	{
		public void Init(HttpApplication context)
		{
			IEngine engine = new MediumTrustEngine();
			Context.Initialize(engine);
			Context.Current.Attach(context);
			engine.InitializePlugins();
		}

		public void Dispose()
		{
		}
	}
}
