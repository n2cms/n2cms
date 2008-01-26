using System.Web;
using N2.MediumTrust.Engine;

namespace N2.MediumTrust.Web
{
	public class InitializerModule : IHttpModule
	{
		public void Init(HttpApplication context)
		{
			Context.Initialize(new MediumTrustFactory());
			Context.Instance.Attach(context);
		}

		public void Dispose()
		{
		}
	}
}
